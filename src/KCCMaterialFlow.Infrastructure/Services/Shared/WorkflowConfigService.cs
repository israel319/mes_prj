using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion du workflow d'approbation configurable.
/// Priorité de résolution :
///   1. Config spécifique au département (BonType + DepartementCode)
///   2. Config générique (BonType, DepartementCode = null)
///   3. Défaut métier codé : SUPERINTENDENT → GM → OPJ → IDENTIFICATION
/// </summary>
public sealed class WorkflowConfigService : IWorkflowConfigService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _factory;

    private static readonly IReadOnlyList<WorkflowEtapeConfig> _defaultWorkflow =
        new List<WorkflowEtapeConfig>
        {
            new() { BonType = "DEFAULT", OrdreEtape = 1, RoleCode = "SUPERINTENDENT", NomEtape = "Superintendent", EstActif = true },
            new() { BonType = "DEFAULT", OrdreEtape = 2, RoleCode = "GM",             NomEtape = "General Manager",  EstActif = true },
            new() { BonType = "DEFAULT", OrdreEtape = 3, RoleCode = "OPJ",            NomEtape = "OPJ",              EstActif = true },
            new() { BonType = "DEFAULT", OrdreEtape = 4, RoleCode = "IDENTIFICATION", NomEtape = "Identification",   EstActif = true },
        };

    public WorkflowConfigService(IDbContextFactory<KCCMaterialFlowDbContext> factory)
        => _factory = factory;

    // ─── Résolution runtime ──────────────────────────────────────────────────

    public async Task<IReadOnlyList<WorkflowEtapeConfig>> GetResolvedWorkflowEtapesAsync(
        string bonType, string? raisonSortieCode, string? agentDepartementCode = null,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        // 1. Exception cross-département : raison + département de l'agent demandeur
        if (!string.IsNullOrWhiteSpace(agentDepartementCode) && raisonSortieCode != null)
        {
            var crossEtapes = await ctx.WorkflowEtapeConfigs
                .Where(x => x.BonType == bonType && x.EstActif &&
                            x.RaisonSortieCode == raisonSortieCode &&
                            x.DepartementCode == agentDepartementCode)
                .OrderBy(x => x.OrdreEtape)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            if (crossEtapes.Count > 0) return crossEtapes;
        }

        // 2. Config spécifique à la raison (aucun département)
        if (raisonSortieCode != null)
        {
            var raisonEtapes = await ctx.WorkflowEtapeConfigs
                .Where(x => x.BonType == bonType && x.EstActif &&
                            x.RaisonSortieCode == raisonSortieCode && x.DepartementCode == null)
                .OrderBy(x => x.OrdreEtape)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            if (raisonEtapes.Count > 0) return raisonEtapes;
        }

        // 3. Config générique (aucune raison, aucun département)
        var genericEtapes = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.EstActif &&
                        x.RaisonSortieCode == null && x.DepartementCode == null)
            .OrderBy(x => x.OrdreEtape)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return genericEtapes.Count > 0 ? genericEtapes : _defaultWorkflow;
    }

    public async Task<IReadOnlyList<WorkflowEtapeConfig>> GetResolvedWorkflowEtapesForDepartementAsync(
        string bonType, string? departementCode, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        // 1. Config spécifique département
        if (!string.IsNullOrWhiteSpace(departementCode))
        {
            var deptEtapes = await ctx.WorkflowEtapeConfigs
                .Where(x => x.BonType == bonType && x.EstActif &&
                            x.DepartementCode == departementCode &&
                            x.RaisonSortieCode == null)
                .OrderBy(x => x.OrdreEtape)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (deptEtapes.Count > 0) return deptEtapes;
        }

        // 2. Config générique
        var genericEtapes = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.EstActif &&
                        x.DepartementCode == null && x.RaisonSortieCode == null)
            .OrderBy(x => x.OrdreEtape)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return genericEtapes.Count > 0 ? genericEtapes : _defaultWorkflow;
    }

    // ─── Lecture admin ───────────────────────────────────────────────────────

    public async Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesForAdminAsync(
        string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        return await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType &&
                        x.DepartementCode == null &&
                        (raisonSortieCode == null ? x.RaisonSortieCode == null : x.RaisonSortieCode == raisonSortieCode))
            .OrderBy(x => x.OrdreEtape)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesForDepartementAsync(
        string bonType, string? departementCode, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        return await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType &&
                        x.DepartementCode == departementCode &&
                        x.RaisonSortieCode == null)
            .OrderBy(x => x.OrdreEtape)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkflowContextSummary>> GetWorkflowSummaryAsync(
        string bonType, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        var configs = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.DepartementCode == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var raisons = await ctx.RaisonsSortie.AsNoTracking().ToListAsync(cancellationToken);

        var result = new List<WorkflowContextSummary>();

        // Config générique (null raison)
        var generiques = configs.Where(x => x.RaisonSortieCode == null).OrderBy(x => x.OrdreEtape).ToList();
        result.Add(new WorkflowContextSummary
        {
            RaisonSortieCode = null,
            RaisonSortieNom = "Générique (toutes raisons)",
            EstPersonnalise = generiques.Count > 0,
            NombreEtapesBD = generiques.Count,
            NombreEtapesResolu = generiques.Count > 0 ? generiques.Count : _defaultWorkflow.Count,
            Source = generiques.Count > 0 ? "Base de données" : "Défaut métier",
        });

        // Par raison de sortie
        foreach (var raison in raisons.OrderBy(x => x.Nom))
        {
            var etapes = configs.Where(x => x.RaisonSortieCode == raison.Code).OrderBy(x => x.OrdreEtape).ToList();
            result.Add(new WorkflowContextSummary
            {
                RaisonSortieCode = raison.Code,
                RaisonSortieNom = raison.Nom,
                EstPersonnalise = etapes.Count > 0,
                NombreEtapesBD = etapes.Count,
                NombreEtapesResolu = etapes.Count > 0 ? etapes.Count : (generiques.Count > 0 ? generiques.Count : _defaultWorkflow.Count),
                Source = etapes.Count > 0 ? "Spécifique raison" : (generiques.Count > 0 ? "Hérité générique" : "Défaut métier"),
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<WorkflowDepartementSummary>> GetDepartementWorkflowSummaryAsync(
        string bonType, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        var configs = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.EstActif)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var depts = await ctx.AllEmployees
            .Where(x => x.DepartementCode != null)
            .GroupBy(x => new { x.DepartementCode, x.Departement })
            .Select(g => new { g.Key.DepartementCode, g.Key.Departement, Count = g.Count() })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new List<WorkflowDepartementSummary>();

        // Ligne générique (null dept)
        var generiques = configs.Where(x => x.DepartementCode == null).OrderBy(x => x.OrdreEtape).ToList();
        result.Add(new WorkflowDepartementSummary
        {
            DepartementCode = null,
            DepartementNom = "Générique (tous départements)",
            EstPersonnalise = generiques.Count > 0,
            NombreEmployes = await ctx.AllEmployees.CountAsync(cancellationToken),
            EtapesConfigurees = generiques.Select(x => x.RoleCode).ToList(),
            Source = generiques.Count > 0 ? "Base de données" : "Défaut métier",
        });

        // Par département
        foreach (var dept in depts.OrderBy(x => x.Departement ?? x.DepartementCode))
        {
            var deptEtapes = configs.Where(x => x.DepartementCode == dept.DepartementCode).OrderBy(x => x.OrdreEtape).ToList();
            result.Add(new WorkflowDepartementSummary
            {
                DepartementCode = dept.DepartementCode,
                DepartementNom = dept.Departement ?? dept.DepartementCode ?? "Inconnu",
                EstPersonnalise = deptEtapes.Count > 0,
                NombreEmployes = dept.Count,
                EtapesConfigurees = deptEtapes.Count > 0 ? deptEtapes.Select(x => x.RoleCode).ToList() : generiques.Select(x => x.RoleCode).ToList(),
                Source = deptEtapes.Count > 0 ? "Spécifique département" : (generiques.Count > 0 ? "Hérité générique" : "Défaut métier"),
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<DepartementInfo>> GetDepartementsAsync(
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        return await ctx.AllEmployees
            .Where(x => x.DepartementCode != null)
            .GroupBy(x => new { x.DepartementCode, x.Departement })
            .Select(g => new DepartementInfo
            {
                Code = g.Key.DepartementCode!,
                Nom = g.Key.Departement ?? g.Key.DepartementCode!,
                NombreEmployes = g.Count(),
            })
            .OrderBy(x => x.Nom)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    // ─── Sauvegarde / suppression ────────────────────────────────────────────

    public async Task SaveWorkflowEtapesAsync(
        string bonType, string? raisonSortieCode,
        IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        // Supprimer les existants
        var existing = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.DepartementCode == null &&
                        (raisonSortieCode == null ? x.RaisonSortieCode == null : x.RaisonSortieCode == raisonSortieCode))
            .ToListAsync(cancellationToken);
        ctx.WorkflowEtapeConfigs.RemoveRange(existing);

        // Ajouter les nouvelles
        var now = DateTime.Now;
        foreach (var e in etapes)
        {
            e.BonType = bonType;
            e.RaisonSortieCode = raisonSortieCode;
            e.DepartementCode = null;
            e.DateModification = now;
            e.ModifieParLogin = modifiedByLogin;
            e.EstActif = true;
            ctx.WorkflowEtapeConfigs.Add(e);
        }

        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveWorkflowEtapesForDepartementAsync(
        string bonType, string? departementCode,
        IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        var existing = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.DepartementCode == departementCode && x.RaisonSortieCode == null)
            .ToListAsync(cancellationToken);
        ctx.WorkflowEtapeConfigs.RemoveRange(existing);

        var now = DateTime.Now;
        foreach (var e in etapes)
        {
            e.BonType = bonType;
            e.DepartementCode = departementCode;
            e.RaisonSortieCode = null;
            e.DateModification = now;
            e.ModifieParLogin = modifiedByLogin;
            e.EstActif = true;
            ctx.WorkflowEtapeConfigs.Add(e);
        }

        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWorkflowEtapesAsync(
        string bonType, string? raisonSortieCode, string? modifiedByLogin,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        var existing = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.DepartementCode == null &&
                        (raisonSortieCode == null ? x.RaisonSortieCode == null : x.RaisonSortieCode == raisonSortieCode))
            .ToListAsync(cancellationToken);
        ctx.WorkflowEtapeConfigs.RemoveRange(existing);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWorkflowEtapesForDepartementAsync(
        string bonType, string? departementCode, string? modifiedByLogin,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        var existing = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType && x.DepartementCode == departementCode && x.RaisonSortieCode == null)
            .ToListAsync(cancellationToken);
        ctx.WorkflowEtapeConfigs.RemoveRange(existing);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieAsync(
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        return await ctx.RaisonsSortie
            .OrderBy(x => x.Nom)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    // ─── Exception cross-département ─────────────────────────────────────────

    public async Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesCrossAsync(
        string bonType, string? raisonSortieCode, string departementCode,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        // Priorité : (raison spécifique + dept) > (générique dept sans raison)
        if (raisonSortieCode != null)
        {
            var specific = await ctx.WorkflowEtapeConfigs
                .Where(x => x.BonType == bonType &&
                            x.DepartementCode == departementCode &&
                            x.RaisonSortieCode == raisonSortieCode)
                .OrderBy(x => x.OrdreEtape)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (specific.Count > 0) return specific;

            // Pas d'override spécifique → on retombe sur l'override générique du dept
            return await ctx.WorkflowEtapeConfigs
                .Where(x => x.BonType == bonType &&
                            x.DepartementCode == departementCode &&
                            x.RaisonSortieCode == null)
                .OrderBy(x => x.OrdreEtape)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        return await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType &&
                        x.DepartementCode == departementCode &&
                        x.RaisonSortieCode == null)
            .OrderBy(x => x.OrdreEtape)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DepartementInfo>> GetDepartementOverridesForRaisonAsync(
        string bonType, string? raisonSortieCode,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        // Codes des départements qui ont au moins une étape pour cette combinaison.
        // On remonte :  (1) les overrides spécifiques à cette raison
        //               (2) les overrides génériques (sans raison) qui s'appliquent à tous les types
        var deptCodes = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType &&
                        x.DepartementCode != null &&
                        (x.RaisonSortieCode == null || x.RaisonSortieCode == raisonSortieCode))
            .Select(x => x.DepartementCode!)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (deptCodes.Count == 0) return Array.Empty<DepartementInfo>();

        // Les codes de vraies départements (hors "*" qui est la branche universelle)
        var realCodes = deptCodes.Where(c => c != "*").ToList();
        var result = new List<DepartementInfo>();

        // Branche spécifique universelle (code "*") : pas de jointure employés nécessaire
        if (deptCodes.Contains("*"))
            result.Add(new DepartementInfo { Code = "*", Nom = "Tous les départements", NombreEmployes = 0 });

        if (realCodes.Count > 0)
        {
            var fromDb = await ctx.AllEmployees
                .Where(x => x.DepartementCode != null && realCodes.Contains(x.DepartementCode!))
                .GroupBy(x => new { x.DepartementCode, x.Departement })
                .Select(g => new DepartementInfo
                {
                    Code = g.Key.DepartementCode!,
                    Nom = g.Key.Departement ?? g.Key.DepartementCode!,
                    NombreEmployes = g.Count(),
                })
                .OrderBy(x => x.Nom)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            result.AddRange(fromDb);
        }

        return result;
    }

    public async Task SaveWorkflowEtapesCrossAsync(
        string bonType, string? raisonSortieCode, string departementCode,
        IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin,
        CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);

        var existing = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType &&
                        x.DepartementCode == departementCode &&
                        (raisonSortieCode == null ? x.RaisonSortieCode == null : x.RaisonSortieCode == raisonSortieCode))
            .ToListAsync(cancellationToken);
        ctx.WorkflowEtapeConfigs.RemoveRange(existing);

        var now = DateTime.Now;
        foreach (var e in etapes)
        {
            e.BonType = bonType;
            e.RaisonSortieCode = raisonSortieCode;
            e.DepartementCode = departementCode;
            e.DateModification = now;
            e.ModifieParLogin = modifiedByLogin;
            e.EstActif = true;
            ctx.WorkflowEtapeConfigs.Add(e);
        }

        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWorkflowEtapesCrossAsync(
        string bonType, string? raisonSortieCode, string departementCode,
        string? modifiedByLogin, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        var existing = await ctx.WorkflowEtapeConfigs
            .Where(x => x.BonType == bonType &&
                        x.DepartementCode == departementCode &&
                        (raisonSortieCode == null ? x.RaisonSortieCode == null : x.RaisonSortieCode == raisonSortieCode))
            .ToListAsync(cancellationToken);
        ctx.WorkflowEtapeConfigs.RemoveRange(existing);
        await ctx.SaveChangesAsync(cancellationToken);
    }
}
