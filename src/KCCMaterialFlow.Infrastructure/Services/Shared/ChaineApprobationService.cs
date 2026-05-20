using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Construit la chaîne d'approbateurs v2 :
/// SUPT → GM → [ITDept | EnvDept selon TypeMateriel]
/// → OPJ (par site, fallback global) → Identification (OR).
///
/// Source de hiérarchie = T_AllEmployees (référentiel).
/// Source des étapes spéciales = T_WorkflowApprobateurSpecial (admin paramétrable).
/// Liaison AllEmployee↔Employee local via UserName↔Login (insensible à la casse, normalisation
/// du préfixe domaine "DOMAIN\").
/// </summary>
public sealed class ChaineApprobationService : IChaineApprobationService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _factory;
    private readonly ILogger<ChaineApprobationService> _logger;

    public ChaineApprobationService(
        IDbContextFactory<KCCMaterialFlowDbContext> factory,
        ILogger<ChaineApprobationService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ChaineApprobationResult> ConstruireChaineAsync(
        int demandeurEmployeeId,
        string? descriptionMateriel,
        int? siteId,
        WorkflowRoutage routage = WorkflowRoutage.Standard,
        CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct);

        // 1. Charger le demandeur (Employee local).
        var demandeur = await ctx.Employees.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == demandeurEmployeeId, ct)
            ?? throw new InvalidOperationException($"Employé Id={demandeurEmployeeId} introuvable.");

        if (string.IsNullOrWhiteSpace(demandeur.Matricule))
            throw new InvalidOperationException(
                $"L'employé {demandeur.NomComplet} n'a pas de Matricule défini ; chaîne d'approbation impossible.");

        // 2. Trouver sa fiche Glencore par Matricule ↔ EmployeeCode.
        var demandeurGlencore = await FindGlencoreByMatriculeAsync(ctx, demandeur.Matricule!, ct)
            ?? throw new InvalidOperationException(
                $"Aucune fiche Glencore trouvée pour le matricule '{demandeur.Matricule}' ({demandeur.NomComplet}). " +
                "Vérifier l'import T_AllEmployees ou le Matricule de l'employé.");

        // 3. Charger les approbateurs spéciaux actifs.
        var specials = await ctx.WorkflowApprobateursSpeciaux
            .Include(x => x.Employee)
            .Where(x => x.EstActif)
            .OrderBy(x => x.Type).ThenBy(x => x.Ordre)
            .AsNoTracking()
            .ToListAsync(ct);

        var etapes = new List<ChaineApprobationEtape>();
        int ordre = 1;
        int? lastEmployeeId = demandeur.Id;

        // ── Étape 1 : SuperIntendent (prioritaire) OU Manager (fallback) ─────
        // Règle : Superintendent si renseigné, sinon Manager. Jamais les deux.
        if (!string.IsNullOrWhiteSpace(demandeurGlencore.SuperIntendentEmployeeCode))
        {
            var superintendent = await ResolveLocalEmployeeAsync(
                ctx,
                demandeurGlencore.SuperIntendentEmployeeCode!,
                demandeurGlencore.SuperIntendentEmployeeDisplay,
                ct);
            AddEtapeIfNew(etapes, ref ordre, ref lastEmployeeId,
                EtapeApprobationKind.SuperIntendent, superintendent);
        }
        else if (!string.IsNullOrWhiteSpace(demandeurGlencore.ManagerHodEmployeeCode))
        {
            var manager = await ResolveLocalEmployeeAsync(
                ctx,
                demandeurGlencore.ManagerHodEmployeeCode!,
                demandeurGlencore.ManagerHodEmployeeDisplay,
                ct);

            // Si ce Manager est référencé comme Superintendent par d'autres employés Glencore,
            // utiliser le kind SuperIntendent pour que le bon affiche "Superintendent" correctement.
            var isActuallySuperintendent = await ctx.AllEmployees
                .AsNoTracking()
                .AnyAsync(g => g.SuperIntendentEmployeeCode == demandeurGlencore.ManagerHodEmployeeCode, ct);

            var kindEtape = isActuallySuperintendent
                ? EtapeApprobationKind.SuperIntendent
                : EtapeApprobationKind.ReportsTo;

            AddEtapeIfNew(etapes, ref ordre, ref lastEmployeeId, kindEtape, manager);
        }
        else
        {
            _logger.LogWarning(
                "Demandeur {EmployeeCode} sans SuperIntendent ni Manager Glencore — étape 1 ignorée, chaîne commence au GM.",
                demandeurGlencore.EmployeeCode);
        }

        // ── Étape 2 : GM ─────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(demandeurGlencore.GmEmployeeCode))
        {
            var gm = await ResolveLocalEmployeeAsync(
                ctx,
                demandeurGlencore.GmEmployeeCode!,
                demandeurGlencore.GmEmployeeDisplay,
                ct);
            AddEtapeIfNew(etapes, ref ordre, ref lastEmployeeId,
                EtapeApprobationKind.GM, gm);
        }
        else
        {
            _logger.LogWarning(
                "Demandeur {EmployeeCode} sans GmEmployeeCode Glencore — étape GM ignorée.",
                demandeurGlencore.EmployeeCode);
        }

        // ── Étape 3 (conditionnelle) : IT ou Environnement — OBLIGATOIRE si routage concerné ───
        if (routage == WorkflowRoutage.IT)
        {
            var candidates = specials.Where(x => x.Type == TypeApprobateurSpecial.IT).ToList();
            WorkflowApprobateurSpecial? dept = null;
            if (siteId is int itSite) dept = candidates.FirstOrDefault(x => x.SiteId == itSite);
            dept ??= candidates.FirstOrDefault(x => x.SiteId == null);
            if (dept is null)
                throw new InvalidOperationException(
                    siteId is null
                        ? "Aucun approbateur IT configuré (voir /admin/workflow-approbateurs). " +
                          "Veuillez désigner un responsable IT avant de soumettre ce bon."
                        : $"Aucun approbateur IT configuré pour le site Id={siteId} ni en global " +
                          "(voir /admin/workflow-approbateurs). Veuillez désigner un responsable IT.");
            AddEtapeIfNew(etapes, ref ordre, ref lastEmployeeId,
                EtapeApprobationKind.ITDepartment,
                dept.EmployeeId, dept.Employee.NomComplet, dept.Employee.Matricule);
        }
        else if (routage == WorkflowRoutage.Environment)
        {
            var candidates = specials.Where(x => x.Type == TypeApprobateurSpecial.Environment).ToList();
            WorkflowApprobateurSpecial? dept = null;
            if (siteId is int envSite) dept = candidates.FirstOrDefault(x => x.SiteId == envSite);
            dept ??= candidates.FirstOrDefault(x => x.SiteId == null);
            if (dept is null)
                throw new InvalidOperationException(
                    siteId is null
                        ? "Aucun approbateur Environnement configuré (voir /admin/workflow-approbateurs). " +
                          "Veuillez désigner un responsable Environnement avant de soumettre ce bon."
                        : $"Aucun approbateur Environnement configuré pour le site Id={siteId} ni en global " +
                          "(voir /admin/workflow-approbateurs). Veuillez désigner un responsable Environnement.");
            AddEtapeIfNew(etapes, ref ordre, ref lastEmployeeId,
                EtapeApprobationKind.EnvironmentDepartment,
                dept.EmployeeId, dept.Employee.NomComplet, dept.Employee.Matricule);
        }

        // ── Étape 4 : OPJ (par site, fallback global) ────────────────
        var opjCandidates = specials
            .Where(x => x.Type == TypeApprobateurSpecial.OPJ)
            .ToList();

        WorkflowApprobateurSpecial? opj = null;
        if (siteId is int sid)
            opj = opjCandidates.FirstOrDefault(x => x.SiteId == sid);
        opj ??= opjCandidates.FirstOrDefault(x => x.SiteId == null);

        if (opj is null)
            throw new InvalidOperationException(
                siteId is null
                    ? "Aucun OPJ global configuré (voir /admin/workflow-approbateurs)."
                    : $"Aucun OPJ configuré pour le site Id={siteId} ni en global (voir /admin/workflow-approbateurs).");

        AddEtapeIfNew(etapes, ref ordre, ref lastEmployeeId,
            EtapeApprobationKind.OPJ,
            opj.EmployeeId, opj.Employee.NomComplet, opj.Employee.Matricule);

        // ── Étape 5 : Identification (OR) ────────────────────────────
        var identification = specials
            .Where(x => x.Type == TypeApprobateurSpecial.Identification)
            .ToList();

        if (identification.Count == 0)
            throw new InvalidOperationException(
                "Aucun approbateur Identification configuré (voir /admin/workflow-approbateurs).");

        var primaryId = identification[0];
        var coApprobateurs = identification.Skip(1).Select(x => x.EmployeeId).ToList();
        etapes.Add(new ChaineApprobationEtape(
            ordre++, EtapeApprobationKind.Identification,
            primaryId.EmployeeId, primaryId.Employee.NomComplet, primaryId.Employee.Matricule, null,
            coApprobateurs));

        _logger.LogInformation(
            "Chaîne v2 construite pour Employee={DemandeurId} (Glencore={EmployeeCode}, Routage={Routage}, Site={Site}) : {Count} étape(s)",
            demandeurEmployeeId, demandeurGlencore.EmployeeCode, routage, siteId, etapes.Count);

        return new ChaineApprobationResult(demandeurEmployeeId, etapes);
    }

    public async Task<ChaineApprobationValidation> ValiderAsync(
        int demandeurEmployeeId,
        string? descriptionMateriel,
        int? siteId,
        WorkflowRoutage routage = WorkflowRoutage.Standard,
        CancellationToken ct = default)
    {
        try
        {
            var result = await ConstruireChaineAsync(demandeurEmployeeId, descriptionMateriel, siteId, routage, ct);
            return result.Etapes.Count == 0
                ? new ChaineApprobationValidation(false, new[] { "Chaîne d'approbation vide." })
                : new ChaineApprobationValidation(true, Array.Empty<string>());
        }
        catch (InvalidOperationException ex)
        {
            return new ChaineApprobationValidation(false, new[] { ex.Message });
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private static void AddEtapeIfNew(
        List<ChaineApprobationEtape> etapes,
        ref int ordre,
        ref int? lastEmployeeId,
        EtapeApprobationKind kind,
        Employee employee)
    {
        if (lastEmployeeId == employee.Id) return;
        etapes.Add(new ChaineApprobationEtape(
            ordre++, kind, employee.Id, employee.NomComplet, employee.Matricule, null,
            Array.Empty<int>()));
        lastEmployeeId = employee.Id;
    }

    private static void AddEtapeIfNew(
        List<ChaineApprobationEtape> etapes,
        ref int ordre,
        ref int? lastEmployeeId,
        EtapeApprobationKind kind,
        int employeeId,
        string nomComplet,
        string? matricule,
        string? login = null)
    {
        if (lastEmployeeId == employeeId) return;
        etapes.Add(new ChaineApprobationEtape(
            ordre++, kind, employeeId, nomComplet, matricule, null,
            Array.Empty<int>()));
        lastEmployeeId = employeeId;
    }

    /// <summary>
    /// Trouve la fiche AllEmployee par Matricule (EmployeeCode). Match insensible à la casse.
    /// </summary>
    private static async Task<AllEmployee?> FindGlencoreByMatriculeAsync(
        KCCMaterialFlowDbContext ctx, string matricule, CancellationToken ct)
    {
        var upper = matricule.ToUpperInvariant();
        return await ctx.AllEmployees.AsNoTracking()
            .Where(g => g.EmployeeCode.ToUpper() == upper)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Résout un Glencore EmployeeCode en Employee local. Auto-crée un Employee
    /// stub si inexistant — nécessaire pour pouvoir
    /// assigner Approbation.ApprobateurId qui pointe sur T_Employees.
    /// </summary>
    private async Task<Employee> ResolveLocalEmployeeAsync(
        KCCMaterialFlowDbContext ctx,
        string glencoreEmployeeCode,
        string? fallbackDisplay,
        CancellationToken ct)
    {
        var glencore = await ctx.AllEmployees.AsNoTracking()
            .FirstOrDefaultAsync(g => g.EmployeeCode == glencoreEmployeeCode, ct)
            ?? throw new InvalidOperationException(
                $"AllEmployee EmployeeCode='{glencoreEmployeeCode}' introuvable dans T_AllEmployees.");

        // Trouver l'Employee local via Matricule ↔ EmployeeCode
        var empCodeUpper = glencore.EmployeeCode.ToUpperInvariant();
        var existing = await ctx.Employees
            .Where(e => e.Matricule != null && e.Matricule.ToUpper() == empCodeUpper)
            .FirstOrDefaultAsync(ct);

        if (existing is not null)
            return existing;

        // Sinon : auto-créer un stub Employee
        var displayName = !string.IsNullOrWhiteSpace(fallbackDisplay)
            ? fallbackDisplay
            : $"{glencore.FirstName} {glencore.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = glencore.EmployeeCode;

        var stub = new Employee
        {
            Matricule = glencore.EmployeeCode,
            NomComplet = displayName,
            DisplayName = displayName,
            Prenom = glencore.FirstName,
            Nom = glencore.LastName,
            Email = glencore.Mail,
            DepartementNom = glencore.Departement,
            EstInterne = true,
            DateCreation = DateTime.Now
        };

        ctx.Employees.Add(stub);
        await ctx.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Employee local auto-créé pour Glencore {EmployeeCode} ({Display}) — Id={Id}",
            glencore.EmployeeCode, displayName, stub.Id);

        return stub;
    }
}
