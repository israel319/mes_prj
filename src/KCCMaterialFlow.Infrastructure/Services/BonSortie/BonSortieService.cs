using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonSortie.DTOs;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.BonSortie;

/// <summary>
/// Implémentation du service métier pour les Bons de Sortie.
/// Délègue les opérations de données au repository (Clean Architecture).
/// </summary>
public class BonSortieService : IBonSortieService
{
    private readonly IBonSortieRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IWorkflowService _workflowService;
    private readonly IWorkflowConfigService _workflowConfigService;
    private readonly IQRCodeService _qrCodeService;
    private readonly IEmailNotificationService _emailService;
    private readonly IBonEntreeLockService _bonEntreeLockService;
    private readonly INotificationRejetService _notificationRejetService;
    private readonly ICategorieSortieService _categorieSortieService;
    private readonly IChaineApprobationService _chaineApprobationService;
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<BonSortieService> _logger;

    public BonSortieService(
        IBonSortieRepository repository,
        ICurrentUserService currentUserService,
        IWorkflowService workflowService,
        IWorkflowConfigService workflowConfigService,
        IQRCodeService qrCodeService,
        IEmailNotificationService emailService,
        IBonEntreeLockService bonEntreeLockService,
        INotificationRejetService notificationRejetService,
        ICategorieSortieService categorieSortieService,
        IChaineApprobationService chaineApprobationService,
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<BonSortieService> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _workflowService = workflowService;
        _workflowConfigService = workflowConfigService;
        _qrCodeService = qrCodeService;
        _emailService = emailService;
        _bonEntreeLockService = bonEntreeLockService;
        _notificationRejetService = notificationRejetService;
        _categorieSortieService = categorieSortieService;
        _chaineApprobationService = chaineApprobationService;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    #region CRUD Operations

    public async Task<BonSortieResult> CreateExterneAsync(CreateBonSortieExterneRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Récupérer le code de la raison si RaisonId est fourni
            string? raisonCode = null;
            if (request.RaisonId.HasValue)
            {
                raisonCode = await _repository.GetRaisonSortieCodeByIdAsync(request.RaisonId.Value, cancellationToken);
            }

            // Description libre du matériel — plus de dépendance à un enum TypeMateriel
            var descriptionMateriel = request.DescriptionMateriel;
            var resolvedRaisonCode = raisonCode ?? request.RaisonSortieCode;

            // TypeMateriel enum supprimé. BonEntree est toujours obligatoire.
            if (!request.BonEntreeAssocieId.HasValue)
            {
                return BonSortieResult.Fail("Un bon d'entrée associé est obligatoire");
            }

            // BSM-031: Vérifier que le bon d'entrée existe et est disponible
            var availabilityResult = await _bonEntreeLockService.CheckAvailabilityAsync(request.BonEntreeAssocieId.Value, cancellationToken);
            if (!availabilityResult.IsAvailable)
            {
                return BonSortieResult.Fail($"Bon d'entrée non disponible: {availabilityResult.ErrorMessage}");
            }

            _logger.LogInformation("BSM-031: BEM {BemNumero} ({Compagnie}) validé pour liaison avec BSM",
                availabilityResult.NumeroReference, availabilityResult.NomCompagnie);

            var coherenceError = await ValidateBonEntreeMotifCoherenceAsync(
                request.BonEntreeAssocieId.Value,
                resolvedRaisonCode,
                cancellationToken);
            if (!string.IsNullOrWhiteSpace(coherenceError))
            {
                return BonSortieResult.Fail(coherenceError);
            }

            var numeroReference = await _repository.GenerateNextNumeroAsync("BSE", cancellationToken);

            // Utiliser le factory method du Domain
            var createResult = BonSortieExterne.Create(
                nomDemandeur: request.NomDemandeur,
                fonctionDemandeur: request.FonctionDemandeur,
                departementDemandeur: request.DepartementDemandeur,
                createdByLogin: _currentUserService.GetUserLogin(),
                motifSortie: request.MotifSortie,
                provenance: request.Provenance,
                destination: request.Destination,
                dateExpiration: request.DateExpiration,
                nomDestinataire: request.NomDestinataire,
                descriptionMateriel: descriptionMateriel,
                bonEntreeAssocieId: request.BonEntreeAssocieId,
                raisonSortieCode: resolvedRaisonCode,
                description: request.Description,
                adresseDestination: request.AdresseDestination,
                numeroVehicule: request.NumeroVehicule,
                nomChauffeur: request.NomChauffeur,
                telephoneChauffeur: request.TelephoneChauffeur);

            if (createResult.IsFailure)
                return BonSortieResult.Fail(createResult.Error.Message);

            var bonSortie = createResult.Value;
            bonSortie.SetNumeroReference(numeroReference);

            // v2 — Site KCC + RequestedFor (chaîne d'approbation par site/département)
            bonSortie.SiteId = request.SiteId;
            bonSortie.RequestedForEmployeeCode = request.RequestedForEmployeeCode;
            bonSortie.RequestedForDisplay = request.RequestedForDisplay;
            bonSortie.RequestedForDepartement = request.RequestedForDepartement;
            bonSortie.SiteManager = request.SiteManager;
            bonSortie.TypeMaterielSortieId = request.TypeMaterielSortieId;

            // Ajouter les matériels via le domain method
            foreach (var mat in request.Materiels)
            {
                var materiel = new MaterielSortie(
                    bonId: 0, // sera défini par EF après insert
                    codeProduitSerial: mat.CodeProduitSerial,
                    designation: mat.Designation,
                    quantite: mat.Quantite,
                    remarque: null);
                bonSortie.AjouterMateriel(materiel);
            }

            var created = await _repository.AddAsync(bonSortie, cancellationToken);

            // Historique
            await AddHistoryAsync(created.Id, ActionBonSortie.Creation, null, "Draft",
                $"Création du bon de sortie externe {numeroReference}", cancellationToken);

            _logger.LogInformation("Bon de sortie externe {Numero} créé par {User}",
                numeroReference, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(created, "Bon de sortie externe créé avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du bon de sortie externe");
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<BonSortieResult> CreateInterneAsync(CreateBonSortieInterneRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            string? raisonCode = null;
            if (request.RaisonId.HasValue)
            {
                raisonCode = await _repository.GetRaisonSortieCodeByIdAsync(request.RaisonId.Value, cancellationToken);
            }

            var descriptionMateriel = request.DescriptionMateriel;
            var resolvedRaisonCode = raisonCode ?? request.RaisonSortieCode;

            if (request.BonEntreeAssocieId.HasValue)
            {
                var coherenceError = await ValidateBonEntreeMotifCoherenceAsync(
                    request.BonEntreeAssocieId.Value,
                    resolvedRaisonCode,
                    cancellationToken);
                if (!string.IsNullOrWhiteSpace(coherenceError))
                {
                    return BonSortieResult.Fail(coherenceError);
                }
            }

            var numeroReference = await _repository.GenerateNextNumeroAsync("BSI", cancellationToken);

            var createResult = BonSortieInterne.Create(
                nomDemandeur: request.NomDemandeur,
                fonctionDemandeur: request.FonctionDemandeur,
                departementDemandeur: request.DepartementDemandeur,
                createdByLogin: _currentUserService.GetUserLogin(),
                motifSortie: request.MotifSortie,
                provenance: request.Provenance,
                destination: request.Destination,
                dateExpiration: request.DateExpiration,
                descriptionMateriel: descriptionMateriel,
                bonEntreeAssocieId: request.BonEntreeAssocieId,
                raisonSortieCode: resolvedRaisonCode,
                description: request.Description,
                departementOrigine: request.DepartementOrigine,
                fonctionReceveur: request.FonctionReceveur,
                emailReceveur: request.EmailReceveur,
                localisationDestination: request.LocalisationDestination,
                dateTransfertPrevue: request.DateTransfertPrevue);

            if (createResult.IsFailure)
                return BonSortieResult.Fail(createResult.Error.Message);

            var bonSortie = createResult.Value;
            bonSortie.SetNumeroReference(numeroReference);

            // v2 — Site KCC + RequestedFor (chaîne d'approbation par site/département)
            bonSortie.SiteId = request.SiteId;
            bonSortie.RequestedForEmployeeCode = request.RequestedForEmployeeCode;
            bonSortie.RequestedForDisplay = request.RequestedForDisplay;
            bonSortie.RequestedForDepartement = request.RequestedForDepartement;
            bonSortie.SiteManager = request.SiteManager;
            bonSortie.TypeMaterielSortieId = request.TypeMaterielSortieId;

            foreach (var mat in request.Materiels)
            {
                var materiel = new MaterielSortie(
                    bonId: 0,
                    codeProduitSerial: mat.CodeProduitSerial,
                    designation: mat.Designation,
                    quantite: mat.Quantite,
                    remarque: null);
                bonSortie.AjouterMateriel(materiel);
            }

            var created = await _repository.AddAsync(bonSortie, cancellationToken);

            await AddHistoryAsync(created.Id, ActionBonSortie.Creation, null, "Draft",
                $"Création du bon de sortie interne {numeroReference}", cancellationToken);

            _logger.LogInformation("Bon de sortie interne {Numero} créé par {User}",
                numeroReference, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(created, "Bon de sortie interne créé avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du bon de sortie interne");
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<BonSortieResult> CreatePretAsync(CreatePretRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            string? raisonCode = null;
            if (request.RaisonId.HasValue)
            {
                raisonCode = await _repository.GetRaisonSortieCodeByIdAsync(request.RaisonId.Value, cancellationToken);
            }
            else
            {
                raisonCode = "PRET";
            }

            var descriptionMateriel = request.DescriptionMateriel;
            var resolvedRaisonCode = raisonCode ?? request.RaisonSortieCode;

            if (request.BonEntreeAssocieId.HasValue)
            {
                var coherenceError = await ValidateBonEntreeMotifCoherenceAsync(
                    request.BonEntreeAssocieId.Value,
                    resolvedRaisonCode,
                    cancellationToken);
                if (!string.IsNullOrWhiteSpace(coherenceError))
                {
                    return BonSortieResult.Fail(coherenceError);
                }
            }

            var numeroReference = await _repository.GenerateNextNumeroAsync("PRT", cancellationToken);

            var dateRetourPrevue = request.DateRetourPrevue ?? DateTime.Now.AddMonths(1);

            var createResult = Pret.Create(
                nomDemandeur: request.NomDemandeur,
                fonctionDemandeur: request.FonctionDemandeur,
                departementDemandeur: request.DepartementDemandeur,
                createdByLogin: _currentUserService.GetUserLogin(),
                motifSortie: request.MotifSortie,
                provenance: request.Provenance,
                destination: request.Destination,
                dateExpiration: request.DateExpiration,
                nomDestinataire: request.NomDestinataire,
                descriptionMateriel: descriptionMateriel,
                dateRetourPrevue: dateRetourPrevue,
                bonEntreeAssocieId: request.BonEntreeAssocieId,
                raisonSortieCode: resolvedRaisonCode,
                description: request.Description,
                adresseDestination: request.AdresseDestination);

            if (createResult.IsFailure)
                return BonSortieResult.Fail(createResult.Error.Message);

            var pret = createResult.Value;
            pret.SetNumeroReference(numeroReference);

            // v2 — Site KCC + RequestedFor
            pret.SiteId = request.SiteId;
            pret.RequestedForEmployeeCode = request.RequestedForEmployeeCode;
            pret.RequestedForDisplay = request.RequestedForDisplay;
            pret.RequestedForDepartement = request.RequestedForDepartement;
            pret.SiteManager = request.SiteManager;

            foreach (var mat in request.Materiels)
            {
                var materiel = new MaterielSortie(
                    bonId: 0,
                    codeProduitSerial: mat.CodeProduitSerial,
                    designation: mat.Designation,
                    quantite: mat.Quantite,
                    remarque: null);
                pret.AjouterMateriel(materiel);
            }

            var created = await _repository.AddAsync(pret, cancellationToken);

            await AddHistoryAsync(created.Id, ActionBonSortie.Creation, null, "Draft",
                $"Création du prêt {numeroReference} - Retour prévu: {dateRetourPrevue:dd/MM/yyyy}", cancellationToken);

            _logger.LogInformation("Prêt {Numero} créé par {User}",
                numeroReference, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(created, "Prêt créé avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du prêt");
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<BonSortieResult> UpdateAsync(UpdateBonSortieRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(request.IdBon, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            if (bon.StatutActuel != "Draft" && bon.StatutActuel != "Returned")
                return BonSortieResult.Fail("Seuls les brouillons et les bons retournés peuvent être modifiés");

            // Mise à jour des champs communs via propriétés publiques sur BonSortie
            if (!string.IsNullOrEmpty(request.MotifSortie))
                bon.MotifSortie = request.MotifSortie;

            if (!string.IsNullOrEmpty(request.Description))
                bon.Description = request.Description;

            if (!string.IsNullOrEmpty(request.Provenance))
                bon.Provenance = request.Provenance;

            if (!string.IsNullOrEmpty(request.Destination))
                bon.Destination = request.Destination;

            if (!string.IsNullOrEmpty(request.FonctionDemandeur))
                bon.FonctionDemandeur = request.FonctionDemandeur;

            if (!string.IsNullOrEmpty(request.DepartementDemandeur))
                bon.DepartementDemandeur = request.DepartementDemandeur;

            if (request.DateExpiration.HasValue)
                bon.DateExpiration = request.DateExpiration.Value;

            // Mise à jour spécifique Prêt: utiliser domain method ProlongerPret
            if (bon is Pret pret && request.DateRetourPrevue.HasValue
                && request.DateRetourPrevue.Value > pret.DateRetourPrevue)
            {
                pret.ProlongerPret(request.DateRetourPrevue.Value);
            }

            // Mise à jour des matériels: supprimer les anciens puis ajouter les nouveaux
            if (request.Materiels != null && request.Materiels.Any())
            {
                // Supprimer les anciens matériels via le repository
                if (bon.Materiels.Any())
                {
                    await _repository.RemoveMaterielsAsync(bon.Id, cancellationToken);
                }

                // Ajouter les nouveaux matériels via le domain method
                foreach (var mat in request.Materiels)
                {
                    var materiel = new MaterielSortie(
                        bonId: bon.Id,
                        codeProduitSerial: mat.CodeProduitSerial,
                        designation: mat.Designation,
                        quantite: mat.Quantite,
                        remarque: null);
                    bon.AjouterMateriel(materiel);
                }
            }

            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.Id, ActionBonSortie.Modification, bon.StatutActuel ?? "Draft", bon.StatutActuel ?? "Draft",
                "Modification du bon de sortie", cancellationToken);

            return BonSortieResult.Ok(bon, "Bon de sortie mis à jour");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du bon {Id}", request.IdBon);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<Domain.Entities.BonSortie?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Domain.Entities.BonSortie?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByNumeroAsync(numeroReference, cancellationToken);
    }

    public async Task<BonSortieSearchResult> GetListAsync(BonSortieFilter filter, CancellationToken cancellationToken = default)
    {
        var userLogin = _currentUserService.GetUserLogin();
        var (items, totalCount) = await _repository.SearchAsync(
            filter.SearchTerm,
            filter.Statut,
            filter.TypeSortie,
            filter.Departement,
            filter.DateDebut,
            filter.DateFin,
            filter.Skip,
            filter.Take,
            userLogin,
            cancellationToken,
            filter.Demandeur);

        return new BonSortieSearchResult
        {
            Items = items,
            TotalCount = totalCount,
            Skip = filter.Skip,
            Take = filter.Take
        };
    }

    public async Task<BonSortieResult> CancelAsync(int id, string motif, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(id, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            var oldStatut = bon.StatutActuel;
            bon.StatutActuel = "Cancelled";

            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.Id, ActionBonSortie.Annulation, oldStatut, "Cancelled",
                $"Annulation: {motif}", cancellationToken);

            return BonSortieResult.Ok(bon, "Bon de sortie annulé");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'annulation du bon {Id}", id);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<BonSortieResult> DeleteDraftAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(id, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            if (bon.StatutActuel != "Draft")
                return BonSortieResult.Fail("Seuls les brouillons peuvent être supprimés");

            var currentUser = _currentUserService.GetUserDisplayName();
            var isAdmin = _currentUserService.IsInAnyRole("Admin");

            if (bon.NomDemandeur != currentUser && !isAdmin)
                return BonSortieResult.Fail("Vous n'êtes pas autorisé à supprimer ce brouillon");

            await _repository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation("Brouillon BSM {Id} ({Numero}) supprimé par {User}",
                id, bon.NumeroReference, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(bon, $"Le brouillon {bon.NumeroReference} a été supprimé");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du brouillon {Id}", id);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    #endregion

    private async Task<string?> ValidateBonEntreeMotifCoherenceAsync(int bonEntreeId, string? raisonSortieCode, CancellationToken cancellationToken)
    {
        // Mapping département-raison désactivé (T_Departements supprimée).
        await Task.CompletedTask;
        _ = bonEntreeId; _ = raisonSortieCode; _ = cancellationToken;
        return null;
    }

    #region User Specific

    public async Task<BonSortieSearchResult> GetMyBonsAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        var userLogin = _currentUserService.GetUserLogin();
        var (items, totalCount) = await _repository.GetByUserAsync(userLogin, skip, take, cancellationToken);

        return new BonSortieSearchResult
        {
            Items = items,
            TotalCount = totalCount,
            Skip = skip,
            Take = take
        };
    }

    public async Task<IReadOnlyList<Domain.Entities.BonSortie>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
    {
        var roles = _currentUserService.GetUserRoles().ToList();
        var isAdmin = roles.Any(r =>
            string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

        var bons = await _repository.GetPendingApprovalsByEmployeeAsync(0, isAdmin, cancellationToken);
        
        _logger.LogInformation("GetPendingApprovalsAsync - Found {TotalCount} pending bons for admin={IsAdmin}", bons.Count, isAdmin);
        
        if (isAdmin) 
        {
            _logger.LogInformation("GetPendingApprovalsAsync - Returning all {Count} bons (user is admin)", bons.Count);
            return bons;
        }

        // Filter by current approver (EmployeeId, or fallback by login/matricule)
        var empId = await ResolveCurrentEmployeeIdAsync(cancellationToken);
        var currentUser = _currentUserService.GetUserLogin()?.ToLowerInvariant() ?? "";
        var currentUserMatricule = _currentUserService.Matricule?.ToLowerInvariant() ?? "";
        
        _logger.LogInformation(
            "GetPendingApprovalsAsync - Filtering for user: EmployeeId={EmpId}, Login={Login}, Matricule={Matricule}",
            empId ?? 0, currentUser, currentUserMatricule);

        // Stratégie 4 : pré-charger les approbateurs spéciaux actifs (OPJ / IDENTIFICATION)
        // pour détecter les bons dont le snapshot est périmé (approbateur changé après soumission).
        await using var ctxSpecial = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var activeSpecials = await ctxSpecial.WorkflowApprobateursSpeciaux
            .Where(x => x.EstActif &&
                (x.Type == TypeApprobateurSpecial.OPJ || x.Type == TypeApprobateurSpecial.Identification))
            .OrderBy(x => x.Ordre)
            .Select(x => new { x.EmployeeId, x.Type, x.SiteId })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var activeOPJs = activeSpecials.Where(x => x.Type == TypeApprobateurSpecial.OPJ).ToList();
        var activeIdentifIds = activeSpecials
            .Where(x => x.Type == TypeApprobateurSpecial.Identification)
            .Select(x => x.EmployeeId)
            .ToHashSet();

        var filtered = bons.Where(b =>
        {
            var current = b.Approbations
                .Where(a => a.Decision == "En attente")
                .OrderBy(a => a.OrdreEtape)
                .FirstOrDefault();
            
            if (current == null)
            {
                _logger.LogDebug("Bon {BonId} has no pending approvals", b.Id);
                return false;
            }

            // Stratégie 1 : correspondance par EmployeeId
            if (current.ApprobateurId.HasValue && empId.HasValue && current.ApprobateurId == empId)
            {
                _logger.LogDebug("Bon {BonId}: MATCH by EmployeeId ({EmpId})", b.Id, empId);
                return true;
            }

            // Stratégie 2 : correspondance par login normalisé (strip préfixe domaine)
            static string NormLogin(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                s = s.Trim().ToLowerInvariant();
                var bs = s.LastIndexOf('\\');
                if (bs >= 0) s = s[(bs + 1)..];
                var at = s.IndexOf('@');
                if (at >= 0) s = s[..at];
                return s;
            }

            var approvalLoginNorm = NormLogin(current.ApprobateurLogin);
            var currentLoginNorm  = NormLogin(currentUser);
            if (!string.IsNullOrEmpty(approvalLoginNorm) && !string.IsNullOrEmpty(currentLoginNorm)
                && approvalLoginNorm == currentLoginNorm)
            {
                _logger.LogDebug("Bon {BonId}: MATCH by login ({Login})", b.Id, currentLoginNorm);
                return true;
            }

            // Stratégie 3 : correspondance par matricule (majuscules)
            var approvalMatricule  = current.ApprobateurMatricule?.Trim().ToUpperInvariant() ?? "";
            var currentMatUpper    = currentUserMatricule.Trim().ToUpperInvariant();
            if (!string.IsNullOrEmpty(approvalMatricule) && !string.IsNullOrEmpty(currentMatUpper)
                && approvalMatricule == currentMatUpper)
            {
                _logger.LogDebug("Bon {BonId}: MATCH by matricule ({Mat})", b.Id, currentMatUpper);
                return true;
            }

            // Stratégie 4 : snapshot périmé — vérifier si l'utilisateur courant est
            // parmi les approbateurs spéciaux actifs pour l'étape OPJ / IDENTIFICATION.
            // Plusieurs approbateurs peuvent coexister pour un même site ; le premier qui approuve fait le job.
            if (empId.HasValue)
            {
                var stepCode = current.CodeEtape?.Trim().ToUpperInvariant();
                if (stepCode == "OPJ")
                {
                    var siteSpecificOPJs = activeOPJs.Where(x => x.SiteId == b.SiteId).ToList();
                    var candidateOPJs = siteSpecificOPJs.Any()
                        ? siteSpecificOPJs
                        : activeOPJs.Where(x => x.SiteId == null).ToList();
                    if (candidateOPJs.Any(x => x.EmployeeId == empId.Value))
                    {
                        _logger.LogDebug("Bon {BonId}: MATCH by active OPJ config (stale snapshot)", b.Id);
                        return true;
                    }
                }
                else if (stepCode == "IDENTIFICATION")
                {
                    if (activeIdentifIds.Contains(empId.Value))
                    {
                        _logger.LogDebug("Bon {BonId}: MATCH by active IDENTIFICATION config (stale snapshot)", b.Id);
                        return true;
                    }
                }
            }

            _logger.LogDebug(
                "Bon {BonId}: No match - AppId={AppId}vs{CurrId}, Login='{ALogin}'vs'{CLogin}', Mat='{AMat}'vs'{CMat}'",
                b.Id, current.ApprobateurId, empId, approvalLoginNorm, currentLoginNorm,
                approvalMatricule, currentMatUpper);
            return false;
        }).ToList();
        
        _logger.LogInformation("GetPendingApprovalsAsync - Filtered {ResultCount} of {TotalCount} bons for user approval", 
            filtered.Count, bons.Count);
        
        return filtered.AsReadOnly();
    }

    public async Task<IReadOnlyList<ApprobationSortie>> GetApprobationsAsync(int bonId, CancellationToken cancellationToken = default)
    {
        var bon = await _repository.GetByIdAsync(bonId, cancellationToken);
        if (bon == null)
            return Array.Empty<ApprobationSortie>();

        return bon.Approbations.OrderBy(a => a.OrdreEtape).ToList();
    }

    public async Task<IReadOnlyList<ReturnedBonInfo>> GetMyReturnedBonsAsync(CancellationToken cancellationToken = default)
    {
        var myBonsResult = await GetMyBonsAsync(0, 100, cancellationToken);

        var returnedBons = new List<ReturnedBonInfo>();

        foreach (var bon in myBonsResult.Items.Where(b => b.StatutActuel == "Draft"))
        {
            // Vérifier s'il y a un historique de retour
            var dernierRetour = bon.Historiques
                .Where(h => h.Action == ActionBonSortie.RetourModification)
                .OrderByDescending(h => h.ActionDate)
                .FirstOrDefault();

            if (dernierRetour != null)
            {
                returnedBons.Add(new ReturnedBonInfo
                {
                    IdBon = bon.Id,
                    NumeroReference = bon.NumeroReference,
                    TypeBon = "BSM",
                    RaisonRetour = dernierRetour.ActionDescription.Replace("Retourné pour modification: ", ""),
                    DateRetour = dernierRetour.ActionDate,
                    AuteurRetour = dernierRetour.ActionByNom ?? ""
                });
            }
        }

        return returnedBons.OrderByDescending(r => r.DateRetour).ToList();
    }

    public async Task<IReadOnlyList<Domain.Entities.BonSortie>> GetApprovedByMeAsync(CancellationToken cancellationToken = default)
    {
        var bons = await _repository.GetApprovedBonsWithApprobationsAsync(cancellationToken);

        var empId = await ResolveCurrentEmployeeIdAsync(cancellationToken);
        var currentUser = _currentUserService.GetUserLogin()?.ToLowerInvariant() ?? "";
        var currentUserMatricule = _currentUserService.Matricule?.ToLowerInvariant() ?? "";

        static string NormLogin(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim().ToLowerInvariant();
            var bs = s.LastIndexOf('\\');
            if (bs >= 0) s = s[(bs + 1)..];
            var at = s.IndexOf('@');
            if (at >= 0) s = s[..at];
            return s;
        }

        return bons.Where(b =>
        {
            var lastApproved = b.Approbations
                .Where(a => a.Decision is "Approuvé" or "Approuve")
                .OrderByDescending(a => a.OrdreEtape)
                .FirstOrDefault();

            if (lastApproved == null) return false;

            if (lastApproved.ApprobateurId.HasValue && empId.HasValue && lastApproved.ApprobateurId == empId)
                return true;

            var approvalLoginNorm = NormLogin(lastApproved.ApprobateurLogin);
            var currentLoginNorm  = NormLogin(currentUser);
            if (!string.IsNullOrEmpty(approvalLoginNorm) && !string.IsNullOrEmpty(currentLoginNorm)
                && approvalLoginNorm == currentLoginNorm)
                return true;

            var approvalMat = lastApproved.ApprobateurMatricule?.Trim().ToUpperInvariant() ?? "";
            var currentMat  = currentUserMatricule.Trim().ToUpperInvariant();
            return !string.IsNullOrEmpty(approvalMat) && !string.IsNullOrEmpty(currentMat)
                   && approvalMat == currentMat;
        }).ToList().AsReadOnly();
    }

    #endregion

    #region Workflow

    public async Task<BonSortieResult> SubmitForApprovalAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(id, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            if (bon.StatutActuel != "Draft")
                return BonSortieResult.Fail("Seuls les brouillons peuvent être soumis");

            var currentLogin = _currentUserService.GetUserLogin();
            if (!string.IsNullOrEmpty(bon.CreatedByLogin) && bon.CreatedByLogin != currentLogin)
                return BonSortieResult.Fail("Vous ne pouvez soumettre que vos propres brouillons");

            if (!bon.Materiels.Any())
                return BonSortieResult.Fail("Ajoutez au moins un matériel avant de soumettre");

            // Supprimer les éventuelles approbations existantes (données pré-migration ou soumission précédente)
            await _repository.DeleteApprobationsAsync(bon.Id, cancellationToken);

            // Créer les étapes d'approbation selon le workflow
            await CreateApprovalStepsAsync(bon, cancellationToken);

            // Recharger le bon pour avoir les nouvelles approbations en mémoire
            bon = (await _repository.GetByIdAsync(bon.Id, cancellationToken))!;

            if (!bon.Approbations.Any())
                return BonSortieResult.Fail("Aucune étape de workflow n'a été configurée pour ce motif");

            var oldStatut = bon.StatutActuel;
            var firstStep = bon.Approbations.OrderBy(a => a.OrdreEtape).First();
            var newStatut = GetStatutForCodeEtape(firstStep.CodeEtape);
            bon.StatutActuel = newStatut;

            await _repository.UpdateAsync(bon, cancellationToken);

            var etapeDescription = $"Soumis à l'étape: {firstStep.NomEtape}";

            await AddHistoryAsync(bon.Id, ActionBonSortie.Soumission, oldStatut, newStatut,
                etapeDescription, cancellationToken);

            _logger.LogInformation("Bon {Numero} soumis pour approbation ({Statut}) par {User}",
                bon.NumeroReference, newStatut, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(bon, $"Bon soumis pour approbation - {etapeDescription}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la soumission du bon {Id}", id);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// Crée les étapes d'approbation pour un bon de sortie.
    /// Les ApprobationSortie sont créées via le repository car la collection est encapsulée.
    /// NomEtape stocke le RoleCode canonisé pour le routing d'autorisation.
    /// </summary>
    private async Task CreateApprovalStepsAsync(Domain.Entities.BonSortie bon, CancellationToken cancellationToken)
    {
        // Chaîne v2 — fondée sur Glencore (ReportsTo → GM → [IT|Env selon TypeMateriel] → OPJ → Identification).
        var creatorLogin = !string.IsNullOrWhiteSpace(bon.CreatedByLogin)
            ? bon.CreatedByLogin
            : _currentUserService.GetUserLogin();

        if (string.IsNullOrWhiteSpace(creatorLogin))
        {
            _logger.LogError("Impossible de construire la chaîne d'approbation : CreatedByLogin et login courant sont vides (bon {BonId}).", bon.Id);
            throw new InvalidOperationException(
                "Impossible de déterminer le créateur du bon : CreatedByLogin et login courant sont vides.");
        }
        var creator = await ResolveOrProvisionEmployeeAsync(creatorLogin, cancellationToken);
        if (creator == null)
        {
            _logger.LogError(
                "Impossible de construire la chaîne d'approbation : aucun Employee local pour CreatedByLogin='{Login}' (bon {BonId}).",
                creatorLogin, bon.Id);
            throw new InvalidOperationException(
                $"Aucun Employee local trouvé pour '{creatorLogin}' (login ou matricule). " +
                "Vérifier l'auto-création depuis Glencore.");
        }

        // Récupérer le WorkflowRoutage associé au type de matériel du bon.
        var routage = WorkflowRoutage.Standard;
        if (bon.TypeMaterielSortieId.HasValue)
        {
            await using var ctxRoutage = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            routage = await ctxRoutage.TypesMateriels
                .AsNoTracking()
                .Where(t => t.Id == bon.TypeMaterielSortieId.Value)
                .Select(t => t.WorkflowRoutage)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var chain = await _chaineApprobationService.ConstruireChaineAsync(
            creator.Id, descriptionMateriel: null, siteId: bon.SiteId, routage: routage, cancellationToken);

        if (chain.Etapes.Count == 0)
            throw new InvalidOperationException($"Chaîne d'approbation vide pour le bon {bon.NumeroReference}.");

        foreach (var etape in chain.Etapes.OrderBy(e => e.Ordre))
        {
            var approbation = new ApprobationSortie(
                bonId: bon.Id,
                ordreEtape: etape.Ordre,
                codeEtape: etape.Kind.ToString().ToUpperInvariant(),
                nomEtape: GetEtapeLabel(etape.Kind),
                approbateurId: etape.EmployeeId,
                approbateurMatricule: etape.EmployeeMatricule,
                approbateurLogin: etape.EmployeeLogin,
                nomApprobateur: etape.EmployeeNomComplet);
            await _repository.AddApprobationAsync(approbation, cancellationToken);
        }

        _logger.LogInformation("Créé {Count} étapes d'approbation pour le bon {Numero}",
            chain.Etapes.Count, bon.NumeroReference);
    }

    private static string GetEtapeLabel(KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind kind) => kind switch
    {
        KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind.SuperIntendent => "Superintendent",
        KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind.ReportsTo => "Manager",
        KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind.GM => "General Manager",
        KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind.ITDepartment => "Département IT",
        KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind.EnvironmentDepartment => "Département Environnement",
        KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind.OPJ => "OPJ",
        KCCMaterialFlow.Application.Common.Interfaces.EtapeApprobationKind.Identification => "Identification",
        _ => kind.ToString()
    };

    public async Task<BonSortieResult> ApproveAsync(BonSortieApprovalRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(request.IdBon, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            var currentStep = bon.Approbations
                .Where(a => a.Decision == "En attente")
                .OrderBy(a => a.OrdreEtape)
                .FirstOrDefault();

            if (currentStep == null)
                return BonSortieResult.Fail("Aucune étape en attente pour ce bon");

            // Résoudre l'identité de l'utilisateur courant avant le refresh (nécessaire pour la sélection multi-approbateurs).
            var currentEmpId = await ResolveCurrentEmployeeIdAsync(cancellationToken);

            // Refresh : synchroniser l'approbateur depuis la config actuelle — si plusieurs approbateurs actifs pour l'étape,
            // le candidat courant est prioritaire (premier qui approuve fait le job).
            await RefreshCurrentStepApproverAsync(bon, currentStep, cancellationToken, currentEmpId);

            // NomEtape contient le RoleCode canonisé
            var requiredRole = currentStep.RoleApprobateur ?? currentStep.NomEtape ?? "";

            // Auth v2 : vérifier l'approbateur désigné (ApprobateurId), avec override Admin.
            var isAdminOverride = _currentUserService.GetUserRoles().Any(r =>
                string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

            if (!isAdminOverride && (currentEmpId == null || currentStep.ApprobateurId != currentEmpId))
            {
                return BonSortieResult.Fail(
                    $"Vous n'êtes pas autorisé à approuver cette étape. Approbateur attendu : {currentStep.NomApprobateur ?? currentStep.NomEtape}.");
            }

            var oldStatut = bon.StatutActuel;

            // Utiliser le domain method sur ApprobationSortie
            currentStep.Approuver(
                _currentUserService.GetUserLogin(),
                _currentUserService.GetUserDisplayName(),
                requiredRole,
                request.Commentaire);

            var nextStep = bon.Approbations
                .Where(a => a.Decision == "En attente")
                .OrderBy(a => a.OrdreEtape)
                .FirstOrDefault();

            var nextStatut = nextStep != null
                ? GetStatutForCodeEtape(nextStep.CodeEtape)
                : "Approved";

            bon.StatutActuel = nextStatut;
            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.Id, ActionBonSortie.Approbation, oldStatut, nextStatut,
                request.Commentaire ?? "Approbation", cancellationToken);

            // BSM-030: Après approbation finale
            if (nextStatut == "Approved")
            {
                // STOCK: Décrémenter les quantités disponibles dans les matériels source du BEM
                if (bon is BonSortieExterne externeForStock && externeForStock.BonEntreeAssocieId.HasValue)
                {
                    try
                    {
                        var materielsASortir = new List<MaterielStockDecrement>();

                        if (bon.Materiels.Any(m => m.MaterielEntreeId.HasValue))
                        {
                            // Cas 1: MaterielEntreeId déjà renseigné sur les lignes BSM
                            materielsASortir = bon.Materiels
                                .Where(m => m.MaterielEntreeId.HasValue)
                                .Select(m => new MaterielStockDecrement
                                {
                                    MaterielEntreeId = m.MaterielEntreeId!.Value,
                                    QuantiteASortir = m.Quantite
                                })
                                .ToList();
                        }
                        else
                        {
                            // Cas 2 (courant): MaterielEntreeId non défini → matcher par CodeProduitSerial depuis le BEM
                            var bemDetails = await _bonEntreeLockService.GetDetailsForSortieAsync(
                                externeForStock.BonEntreeAssocieId.Value, cancellationToken);

                            if (bemDetails != null)
                            {
                                foreach (var matSortie in bon.Materiels)
                                {
                                    var matBem = bemDetails.Materiels.FirstOrDefault(m =>
                                        string.Equals(m.CodeProduitSerial, matSortie.CodeProduitSerial,
                                            StringComparison.OrdinalIgnoreCase));
                                    if (matBem != null)
                                    {
                                        materielsASortir.Add(new MaterielStockDecrement
                                        {
                                            MaterielEntreeId = matBem.IdMateriel,
                                            QuantiteASortir = matSortie.Quantite
                                        });
                                    }
                                    else
                                    {
                                        _logger.LogWarning("BSM {Numero}: matériel '{Serial}' non trouvé dans BEM {BemId} — stock non décrémenté pour cette ligne",
                                            bon.NumeroReference, matSortie.CodeProduitSerial, externeForStock.BonEntreeAssocieId.Value);
                                    }
                                }
                            }
                        }

                        if (materielsASortir.Any())
                        {
                            var stockResult = await _bonEntreeLockService.DecrementStockAsync(materielsASortir, cancellationToken);
                            if (!stockResult.Success)
                                _logger.LogWarning("Échec de la mise à jour du stock pour BSM {Numero}: {Message}",
                                    bon.NumeroReference, stockResult.ErrorMessage);
                            else
                                _logger.LogInformation("Stock décrémenté pour {Count} matériels du BSM {Numero}",
                                    materielsASortir.Count, bon.NumeroReference);
                        }
                        else
                        {
                            _logger.LogWarning("BSM {Numero}: aucun matériel à décrémenter (BEM {BemId})",
                                bon.NumeroReference, externeForStock.BonEntreeAssocieId.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Erreur lors de la décrémentation du stock pour BSM {Numero}",
                            bon.NumeroReference);
                    }
                }

                // BSM-031: Verrouiller le BonEntree associé
                if (bon is BonSortieExterne externe && externe.BonEntreeAssocieId.HasValue)
                {
                    try
                    {
                        await _bonEntreeLockService.LockAsync(
                            externe.BonEntreeAssocieId.Value,
                            bon.Id,
                            bon.NumeroReference,
                            cancellationToken);

                        _logger.LogInformation("BSM-031: BEM {BemId} verrouillé pour BSM {BsmNumero}",
                            externe.BonEntreeAssocieId, bon.NumeroReference);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Échec du verrouillage du BEM {BemId} pour BSM {BsmNumero}",
                            externe.BonEntreeAssocieId, bon.NumeroReference);
                    }
                }

                // BSM-030: Génération du QR Code
                var qrResult = await GenerateQRCodeAsync(bon.Id, cancellationToken);
                if (!qrResult.Success)
                {
                    _logger.LogWarning("Échec de la génération du QR Code pour le bon {Numero}: {Message}",
                        bon.NumeroReference, qrResult.Message);
                }
                else
                {
                    bon = qrResult.Bon!;
                    _logger.LogInformation("QR Code généré automatiquement pour le bon approuvé {Numero}",
                        bon.NumeroReference);
                }
            }

            _logger.LogInformation("Bon {Numero} approuvé ({OldStatut} -> {NewStatut}) par {User}",
                bon.NumeroReference, oldStatut, nextStatut, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(bon, $"Bon approuvé - Nouveau statut: {nextStatut}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'approbation du bon {Id}", request.IdBon);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<BonSortieResult> RejectAsync(RejectRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(request.IdBon, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            var currentStep = bon.Approbations
                .Where(a => a.Decision == "En attente")
                .OrderBy(a => a.OrdreEtape)
                .FirstOrDefault();

            if (currentStep == null)
                return BonSortieResult.Fail("Aucune étape en attente pour ce bon");

            // Résoudre l'identité de l'utilisateur courant avant le refresh (nécessaire pour la sélection multi-approbateurs).
            var currentEmpId = await ResolveCurrentEmployeeIdAsync(cancellationToken);

            // Refresh : synchroniser l'approbateur depuis la config actuelle — si plusieurs approbateurs actifs pour l'étape,
            // le candidat courant est prioritaire (premier qui rejette fait le job).
            await RefreshCurrentStepApproverAsync(bon, currentStep, cancellationToken, currentEmpId);

            var requiredRole = currentStep.RoleApprobateur ?? currentStep.NomEtape ?? "";
            var isAdminOverride = _currentUserService.GetUserRoles().Any(r =>
                string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

            if (!isAdminOverride && (currentEmpId == null || currentStep.ApprobateurId != currentEmpId))
            {
                return BonSortieResult.Fail(
                    $"Vous n'êtes pas autorisé à rejeter cette étape. Approbateur attendu : {currentStep.NomApprobateur ?? currentStep.NomEtape}.");
            }

            var oldStatut = bon.StatutActuel;
            var nomEtape = currentStep.NomEtape;

            // Utiliser le domain method
            currentStep.Rejeter(
                _currentUserService.GetUserLogin(),
                _currentUserService.GetUserDisplayName(),
                requiredRole,
                request.Motif);

            bon.StatutActuel = "Rejected";

            await _repository.UpdateAsync(bon, cancellationToken);

            // BSM-031: Déverrouiller le BonEntree associé si le BSM était approuvé
            if (oldStatut == "Approved" && bon is BonSortieExterne externe && externe.BonEntreeAssocieId.HasValue)
            {
                try
                {
                    await _bonEntreeLockService.UnlockAsync(externe.BonEntreeAssocieId.Value, cancellationToken);
                    _logger.LogInformation("BSM-031: BEM {BemId} déverrouillé suite au rejet de BSM {BsmNumero}",
                        externe.BonEntreeAssocieId, bon.NumeroReference);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Échec du déverrouillage du BEM {BemId}", externe.BonEntreeAssocieId);
                }
            }

            // Enregistrer la notification de rejet
            try
            {
                var bonTypePrefix = bon.NumeroReference.Split('-').FirstOrDefault() ?? "BSM";
                await _notificationRejetService.EnregistrerRejetAsync(
                    bonType: bonTypePrefix,
                    numeroReference: bon.NumeroReference,
                    etapeRejet: nomEtape ?? "Inconnu",
                    approbateurNom: _currentUserService.GetUserDisplayName(),
                    approbateurLogin: _currentUserService.GetUserLogin(),
                    motifRejet: request.Motif ?? "Aucun motif spécifié",
                    demandeurNom: bon.NomDemandeur,
                    cancellationToken);
            }
            catch (Exception notifEx)
            {
                _logger.LogWarning(notifEx, "Échec de l'enregistrement de la notification de rejet pour BSM {Numero}", bon.NumeroReference);
            }

            await AddHistoryAsync(bon.Id, ActionBonSortie.Rejet, oldStatut, "Rejected",
                $"Rejeté: {request.Motif}", cancellationToken);

            _logger.LogInformation("Bon {Numero} rejeté par {User}: {Motif}",
                bon.NumeroReference, _currentUserService.GetUserLogin(), request.Motif);

            return BonSortieResult.Ok(bon, "Bon rejeté");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du rejet du bon {Id}", request.IdBon);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<BonSortieResult> ReturnForModificationAsync(BonSortieApprovalRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(request.IdBon, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            var oldStatut = bon.StatutActuel;

            var currentStep = bon.Approbations
                .Where(a => a.Decision == "En attente")
                .OrderBy(a => a.OrdreEtape)
                .FirstOrDefault();

            if (currentStep == null)
                return BonSortieResult.Fail("Aucune étape en attente pour ce bon");

            // Utiliser le domain method
            currentStep.Retourner(
                _currentUserService.GetUserLogin(),
                _currentUserService.GetUserDisplayName(),
                currentStep.NomEtape ?? "",
                request.Commentaire ?? "");

            // Remettre le bon en brouillon
            bon.StatutActuel = "Draft";

            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.Id, ActionBonSortie.RetourModification, oldStatut, "Draft",
                $"Retourné pour modification: {request.Commentaire}", cancellationToken);

            _logger.LogInformation("Bon {Numero} retourné pour modification par {User}: {Commentaire}",
                bon.NumeroReference, _currentUserService.GetUserLogin(), request.Commentaire);

            return BonSortieResult.Ok(bon, "Bon retourné au demandeur pour modification");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du retour du bon {Id}", request.IdBon);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    private string GetPendingStatusForRoleCode(string? roleCode)
    {
        return CanonicalizeRoleCode(roleCode) switch
        {
            "IT" => "PendingIT",
            "ENVIRONNEMENT" => "PendingEnv",
            "SUPERVISEUR" => "PendingSuperviseur",
            "GM" => "PendingGM",
            "OPJ" => "PendingOPJ",
            "IDENTIFICATION" => "PendingIdentification",
            _ => $"Pending{NormalizeStatusToken(roleCode)}"
        };
    }

    /// <summary>Mapping CodeEtape (kind stable) → StatutActuel pour BSM.</summary>
    private static string GetStatutForCodeEtape(string? codeEtape)
    {
        return (codeEtape ?? "").ToUpperInvariant() switch
        {
            "SUPERINTENDENT" or "REPORTSTO" => "PendingSup",
            "GM" => "PendingGM",
            "ITDEPARTMENT" or "IT" => "PendingIT",
            "ENVIRONMENTDEPARTMENT" or "ENV" => "PendingEnv",
            "OPJ" => "PendingOPJ",
            "IDENTIFICATION" => "PendingIdentification",
            _ => "PendingSup"
        };
    }

    /// <summary>Résoud l'EmployeeId courant via le login Windows ou matricule, avec auto-provisioning Glencore.</summary>
    private async Task<int?> ResolveCurrentEmployeeIdAsync(CancellationToken ct)
    {
        var login = _currentUserService.GetUserLogin();
        var emp = await ResolveOrProvisionEmployeeAsync(login, ct);
        return emp?.Id;
    }

    /// <summary>
    /// Trouve un Employee local par Login Windows OU Matricule (l'impersonation injecte le matricule
    /// comme login). Si introuvable, tente une auto-création depuis le référentiel Glencore.
    /// </summary>
    private async Task<Domain.Entities.Employee?> ResolveOrProvisionEmployeeAsync(string? loginOrMatricule, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(loginOrMatricule)) return null;
        var key = loginOrMatricule.Trim();
        var keyLower = key.ToLowerInvariant();
        var sam = key.Contains('\\') ? key[(key.LastIndexOf('\\') + 1)..] : key;
        var samLower = sam.ToLowerInvariant();

        await using var ctx = await _dbContextFactory.CreateDbContextAsync(ct);

        // 1) Via T_Users : Login → EmployeeId → Employee
        var empId = await ctx.AppUsers.AsNoTracking()
            .Where(u => u.EstActif && (u.Login.ToLower() == keyLower || u.Login.ToLower().EndsWith("\\" + samLower)))
            .Select(u => u.EmployeeId)
            .FirstOrDefaultAsync(ct);
        Domain.Entities.Employee? emp = null;
        if (empId.HasValue)
            emp = await ctx.Set<Domain.Entities.Employee>().FirstOrDefaultAsync(e => e.Id == empId.Value, ct);

        // 2) Fallback : par Matricule
        if (emp == null)
            emp = await ctx.Set<Domain.Entities.Employee>().FirstOrDefaultAsync(e =>
                e.Matricule != null && (e.Matricule.ToLower() == keyLower || e.Matricule.ToLower() == samLower), ct);
        if (emp != null) return emp;

        // 2) Auto-provisioning depuis Glencore : on tente UserName puis EmployeeCode.
        var g = await ctx.Set<Domain.Entities.AllEmployee>().AsNoTracking().FirstOrDefaultAsync(x =>
            (x.UserName != null && (x.UserName.ToLower() == keyLower
                                    || x.UserName.ToLower().EndsWith("\\" + samLower)
                                    || x.UserName.ToLower() == samLower))
            || x.EmployeeCode.ToLower() == keyLower
            || x.EmployeeCode.ToLower() == samLower, ct);
        if (g == null)
        {
            _logger.LogWarning("Résolution Employee : '{Key}' introuvable dans T_Employees ET T_AllEmployees.", key);
            return null;
        }

        var displayName = $"{g.FirstName} {g.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(displayName)) displayName = g.EmployeeCode;

        emp = new Domain.Entities.Employee
        {
            Matricule = g.EmployeeCode,
            NomComplet = displayName,
            DisplayName = displayName,
            Prenom = g.FirstName,
            Nom = g.LastName,
            Email = g.Mail,
            DepartementNom = g.Departement,
            EstInterne = true,
            DateCreation = DateTime.Now
        };
        ctx.Set<Domain.Entities.Employee>().Add(emp);
        await ctx.SaveChangesAsync(ct);
        _logger.LogInformation("Employee local auto-provisionné depuis Glencore : {Code} (Id={Id})", g.EmployeeCode, emp.Id);
        return emp;
    }

    private string GetRequiredRoleFromRoleCode(string? roleCode)
    {
        return CanonicalizeRoleCode(roleCode) switch
        {
            "IT" => "IT",
            "ENVIRONNEMENT" => "Environnement",
            "SUPERVISEUR" => "Superviseur",
            "GM" => "GM",
            "OPJ" => "OPJ",
            "IDENTIFICATION" => "Identification",
            _ => roleCode ?? string.Empty
        };
    }

    private static bool UserHasRequiredRole(IEnumerable<string> userRoles, string requiredRole)
    {
        var requiredCanonical = CanonicalizeRoleCode(requiredRole);
        return userRoles.Any(role => CanonicalizeRoleCode(role) == requiredCanonical);
    }

    private static string CanonicalizeRoleCode(string? roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
        {
            return string.Empty;
        }

        var token = new string(roleCode
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());

        if (token.Contains("ADMIN")) return "ADMIN";
        if (token.Contains("SUPERVISEUR")) return "SUPERVISEUR";
        if (token == "GM" || token.Contains("GENERALMANAGER")) return "GM";
        if (token == "IT" || token.Contains("INFORMATIQUE") || token.Contains("DEPARTEMENTIT") || token.Contains("EQUIPEIT")) return "IT";
        if (token.Contains("ENVIRONNEMENT")) return "ENVIRONNEMENT";
        if (token == "OPJ" || token.Contains("OFFICIERDEPOLICEJUDICIAIRE")) return "OPJ";
        if (token.Contains("IDENTIFICATION")) return "IDENTIFICATION";

        return token;
    }

    private static string NormalizeStatusToken(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "Unknown";
        }

        var chars = input.Where(char.IsLetterOrDigit).ToArray();
        var normalized = new string(chars);
        return string.IsNullOrWhiteSpace(normalized) ? "Unknown" : normalized;
    }

    /// <summary>
    /// Synchronise l'approbateur d'une étape en attente depuis la configuration actuelle.
    /// — Étapes spéciales (OPJ, Identification) : requête directe dans WorkflowApprobateursSpeciaux,
    ///   indépendamment du reste de la chaîne (résistant à la suppression/remplacement de l'approbateur).
    /// — Étapes Glencore (SuperIntendent, GM, ReportsTo) : reconstruction de la chaîne.
    /// La persistance est assurée par UpdateAsync(bon) appelé en fin de ApproveAsync/RejectAsync.
    /// </summary>
    private async Task RefreshCurrentStepApproverAsync(
        KCCMaterialFlow.Domain.Entities.BonSortie bon, ApprobationSortie step, CancellationToken ct, int? candidateEmpId = null)
    {
        if (string.IsNullOrWhiteSpace(step.CodeEtape))
            return;

        var codeUpper = step.CodeEtape.ToUpperInvariant();

        try
        {
            int? newId = null;
            string? newNom = null, newMatricule = null, newLogin = null;

            if (codeUpper is "OPJ" or "IDENTIFICATION")
            {
                // Étapes spéciales : requête directe, pas besoin de reconstruire toute la chaîne.
                var typeSpecial = codeUpper == "OPJ"
                    ? TypeApprobateurSpecial.OPJ
                    : TypeApprobateurSpecial.Identification;

                await using var ctx = await _dbContextFactory.CreateDbContextAsync(ct);
                var specials = await ctx.WorkflowApprobateursSpeciaux
                    .Include(x => x.Employee)
                    .Where(x => x.Type == typeSpecial && x.EstActif)
                    .OrderBy(x => x.Ordre)
                    .AsNoTracking()
                    .ToListAsync(ct);

                WorkflowApprobateurSpecial? match = null;
                if (typeSpecial == TypeApprobateurSpecial.OPJ)
                {
                    // Construire le pool : site-spécifique, sinon global.
                    var pool = (bon.SiteId is int siteId
                        ? specials.Where(x => x.SiteId == siteId).ToList()
                        : new List<WorkflowApprobateurSpecial>());
                    if (!pool.Any())
                        pool = specials.Where(x => x.SiteId == null).ToList();
                    // Si un candidat (qui tente d'approuver) est dans le pool, le prendre en priorité.
                    match = (candidateEmpId.HasValue ? pool.FirstOrDefault(x => x.EmployeeId == candidateEmpId) : null)
                            ?? pool.FirstOrDefault();
                }
                else
                {
                    // IDENTIFICATION : préférer le candidat s'il est dans la liste active.
                    match = (candidateEmpId.HasValue ? specials.FirstOrDefault(x => x.EmployeeId == candidateEmpId) : null)
                            ?? specials.FirstOrDefault();
                }

                if (match == null)
                {
                    _logger.LogWarning(
                        "Aucun approbateur actif pour l'étape {Code} du bon {Ref} — rafraîchissement impossible.",
                        codeUpper, bon.NumeroReference);
                    return;
                }

                newId = match.EmployeeId;
                newNom = match.Employee.NomComplet;
                newMatricule = match.Employee.Matricule;
                var matchUser = await ctx.AppUsers.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.EmployeeId == match.EmployeeId, ct);
                newLogin = matchUser?.Login;
            }
            else
            {
                // Étapes Glencore : reconstruction de la chaîne.
                var creatorLogin = !string.IsNullOrWhiteSpace(bon.CreatedByLogin) ? bon.CreatedByLogin : null;
                if (string.IsNullOrWhiteSpace(creatorLogin)) return;

                var creator = await ResolveOrProvisionEmployeeAsync(creatorLogin, ct);
                if (creator == null) return;

                var routageRefresh = WorkflowRoutage.Standard;
                if (bon.TypeMaterielSortieId.HasValue)
                {
                    await using var ctxR = await _dbContextFactory.CreateDbContextAsync(ct);
                    routageRefresh = await ctxR.TypesMateriels
                        .AsNoTracking()
                        .Where(t => t.Id == bon.TypeMaterielSortieId.Value)
                        .Select(t => t.WorkflowRoutage)
                        .FirstOrDefaultAsync(ct);
                }

                var freshChain = await _chaineApprobationService.ConstruireChaineAsync(
                    creator.Id, descriptionMateriel: null, siteId: bon.SiteId, routage: routageRefresh, ct);

                var freshEtape = freshChain.Etapes.FirstOrDefault(e =>
                    string.Equals(e.Kind.ToString().ToUpperInvariant(), codeUpper,
                        StringComparison.OrdinalIgnoreCase));

                if (freshEtape == null) return;

                newId = freshEtape.EmployeeId;
                newNom = freshEtape.EmployeeNomComplet;
                newMatricule = freshEtape.EmployeeMatricule;
                newLogin = freshEtape.EmployeeLogin;
            }

            if (newId != null && newId != step.ApprobateurId)
            {
                _logger.LogInformation(
                    "Approbateur mis à jour pour étape {Code} du bon {Ref} : {OldId} → {NewId} ({Nom})",
                    codeUpper, bon.NumeroReference, step.ApprobateurId, newId, newNom);

                step.ApprobateurId = newId;
                step.NomApprobateur = newNom;
                step.ApprobateurMatricule = newMatricule;
                step.ApprobateurLogin = newLogin;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Impossible de rafraîchir l'approbateur pour l'étape {Code} du bon {Ref}",
                codeUpper, bon.NumeroReference);
        }
    }

    private static IEnumerable<WorkflowEtapeConfig> BuildDefaultWorkflowEtapes(Domain.Entities.BonSortie bon)
    {
        // LEGACY CODE — TypeMateriel enum supprimé. À refactoriser.
        var etapes = new List<WorkflowEtapeConfig>();
        var ordre = 1;

        // Pour l'instant, workflow standard sans différenciation IT/Env
        etapes.Add(new WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "Superviseur", NomEtape = "Superviseur", EstActif = true });
        etapes.Add(new WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "GM", NomEtape = "General Manager", EstActif = true });
        etapes.Add(new WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "OPJ", NomEtape = "OPJ", EstActif = true });
        etapes.Add(new WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "Identification", NomEtape = "Identification", EstActif = true });

        return etapes;
    }

    #endregion

    #region Prêts

    public async Task<IReadOnlyList<Pret>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetActiveLoansAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pret>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetOverdueLoansAsync(cancellationToken);
    }

    public async Task<BonSortieResult> ReturnLoanAsync(ReturnLoanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(request.IdBon, cancellationToken);
            if (bon is not Pret pret)
                return BonSortieResult.Fail("Ce bon n'est pas un prêt");

            if (pret.EstRetourne)
                return BonSortieResult.Fail("Ce prêt a déjà été retourné");

            // Utiliser le domain method
            var receptionnePar = request.ReceptionnePar ?? _currentUserService.GetUserDisplayName();
            var retourResult = pret.EnregistrerRetour(receptionnePar, request.EtatRetour);
            if (retourResult.IsFailure)
                return BonSortieResult.Fail(retourResult.Error.Message);

            pret.StatutActuel = "Completed";

            await _repository.UpdateAsync(pret, cancellationToken);

            var retardInfo = pret.JoursRetard > 0 ? $" ({pret.JoursRetard} jours de retard)" : "";
            var commentInfo = !string.IsNullOrWhiteSpace(request.Commentaire) ? $". Commentaire: {request.Commentaire}" : "";
            await AddHistoryAsync(pret.Id, ActionBonSortie.RetourPret, "Approved", "Completed",
                $"Matériel retourné{retardInfo}. État: {request.EtatRetour ?? "Non spécifié"}{commentInfo}", cancellationToken);

            // PRÊT: Restaurer le stock et déverrouiller le BEM (les matériels reviennent)
            if (pret.BonEntreeAssocieId.HasValue)
            {
                // 1. Restaurer le stock des matériels
                if (pret.Materiels.Any(m => m.MaterielEntreeId.HasValue))
                {
                    try
                    {
                        var materielsARestituer = pret.Materiels
                            .Where(m => m.MaterielEntreeId.HasValue)
                            .Select(m => new MaterielStockDecrement
                            {
                                MaterielEntreeId = m.MaterielEntreeId!.Value,
                                QuantiteASortir = m.Quantite
                            })
                            .ToList();

                        var stockResult = await _bonEntreeLockService.IncrementStockAsync(materielsARestituer, cancellationToken);
                        if (!stockResult.Success)
                        {
                            _logger.LogWarning("Échec de la restauration du stock pour prêt {Numero}: {Message}",
                                pret.NumeroReference, stockResult.ErrorMessage);
                        }
                        else
                        {
                            _logger.LogInformation("Stock restauré pour {Count} matériels du prêt {Numero}",
                                materielsARestituer.Count, pret.NumeroReference);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Erreur lors de la restauration du stock pour prêt {Numero}",
                            pret.NumeroReference);
                    }
                }

                // 2. Déverrouiller le BEM (pas archiver — les matériels sont de retour)
                try
                {
                    await _bonEntreeLockService.UnlockAsync(
                        pret.BonEntreeAssocieId.Value,
                        cancellationToken);

                    _logger.LogInformation("BEM {BemId} déverrouillé après retour du prêt {PrtNumero}",
                        pret.BonEntreeAssocieId, pret.NumeroReference);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Échec du déverrouillage du BEM {BemId} après retour du prêt {PrtNumero}",
                        pret.BonEntreeAssocieId, pret.NumeroReference);
                }
            }

            _logger.LogInformation("Prêt {Numero} retourné par {User}",
                pret.NumeroReference, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(pret, "Retour enregistré avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du retour du prêt {Id}", request.IdBon);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Pret>> GetLoansExpiringInDaysAsync(int days, CancellationToken cancellationToken = default)
    {
        return await _repository.GetLoansExpiringInDaysAsync(days, cancellationToken);
    }

    public async Task<BonSortieResult> ExtendLoanAsync(ExtendLoanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = _currentUserService.GetUserRoles();
            if (!roles.Contains("Identification") && !roles.Contains("Admin"))
            {
                return BonSortieResult.Fail("Seul le service Identification peut étendre la date de retour d'un prêt");
            }

            var bon = await _repository.GetByIdAsync(request.IdBon, cancellationToken);
            if (bon is not Pret pret)
                return BonSortieResult.Fail("Ce bon n'est pas un prêt");

            if (pret.EstRetourne)
                return BonSortieResult.Fail("Ce prêt a déjà été retourné");

            if (request.NouvelleDateRetour <= pret.DateRetourPrevue)
                return BonSortieResult.Fail("La nouvelle date de retour doit être postérieure à la date actuelle");

            var ancienneDate = pret.DateRetourPrevue;

            // Utiliser le domain method
            var prolongResult = pret.ProlongerPret(request.NouvelleDateRetour);
            if (prolongResult.IsFailure)
                return BonSortieResult.Fail(prolongResult.Error.Message);

            pret.DateExpiration = request.NouvelleDateRetour;

            await _repository.UpdateAsync(pret, cancellationToken);

            await AddHistoryAsync(pret.Id, ActionBonSortie.ExtensionPret, "Approved", "Approved",
                $"Extension de la date de retour: {ancienneDate:dd/MM/yyyy} → {request.NouvelleDateRetour:dd/MM/yyyy}. Motif: {request.Motif}", cancellationToken);

            _logger.LogInformation("Prêt {Numero} prolongé jusqu'au {Date} par {User}",
                pret.NumeroReference, request.NouvelleDateRetour, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(pret, $"Date de retour prolongée jusqu'au {request.NouvelleDateRetour:dd/MM/yyyy}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'extension du prêt {Id}", request.IdBon);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Alertes Email (BSM-028)

    public async Task<int> SendExpirationAlertsAsync(int days = 7, CancellationToken cancellationToken = default)
    {
        try
        {
            var expiringLoans = await _repository.GetLoansExpiringInDaysAsync(days, cancellationToken);
            var alertsSent = 0;

            foreach (var pret in expiringLoans)
            {
                try
                {
                    var joursRestants = (pret.DateRetourPrevue.Date - DateTime.Today).Days;
                    await _emailService.SendPretExpirationAlertAsync(
                        pret.Id,
                        pret.DateRetourPrevue,
                        joursRestants,
                        cancellationToken);
                    alertsSent++;

                    _logger.LogInformation("Alerte envoyée pour prêt {Numero} - J{Days}",
                        pret.NumeroReference, joursRestants >= 0 ? $"-{joursRestants}" : $"+{-joursRestants}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur envoi alerte pour prêt {Id}", pret.Id);
                }
            }

            var overdueLoans = await _repository.GetOverdueLoansAsync(cancellationToken);
            foreach (var pret in overdueLoans)
            {
                try
                {
                    var joursRetard = (DateTime.Today - pret.DateRetourPrevue.Date).Days;
                    await _emailService.SendPretExpirationAlertAsync(
                        pret.Id,
                        pret.DateRetourPrevue,
                        -joursRetard,
                        cancellationToken);
                    alertsSent++;

                    _logger.LogWarning("Alerte RETARD envoyée pour prêt {Numero} - {Days} jours de retard",
                        pret.NumeroReference, joursRetard);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur envoi alerte retard pour prêt {Id}", pret.Id);
                }
            }

            _logger.LogInformation("BSM-028: {AlertsSent} alertes d'expiration envoyées", alertsSent);
            return alertsSent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi des alertes d'expiration");
            throw;
        }
    }

    #endregion

    #region QR Code (BSM-030)

    public async Task<BonSortieResult> GenerateQRCodeAsync(int bonId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(bonId, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            if (bon.StatutActuel != "Approved")
                return BonSortieResult.Fail("Le QR Code ne peut être généré que pour les bons approuvés");

            if (!string.IsNullOrEmpty(bon.QRCodeHash))
            {
                _logger.LogWarning("QR Code déjà généré pour le bon {Numero}", bon.NumeroReference);
                return BonSortieResult.Ok(bon, "Le QR Code existe déjà");
            }

            var bonType = bon switch
            {
                Pret => "PRT",
                BonSortieExterne => "BSE",
                BonSortieInterne => "BSI",
                _ => "BSM"
            };

            var qrResult = _qrCodeService.GenerateQRCode(bon.Id, bonType, bon.NumeroReference);

            // Utiliser le domain method
            bon.SetQRCode(
                data: $"{{\"Id\":{bon.Id},\"Type\":\"{bonType}\",\"Ref\":\"{bon.NumeroReference}\",\"Hash\":\"{qrResult.HashedCode}\"}}",
                base64: qrResult.QRCodeBase64,
                hash: qrResult.HashedCode);

            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.Id, ActionBonSortie.GenerationQR, bon.StatutActuel, bon.StatutActuel,
                $"QR Code généré pour le bon {bon.NumeroReference}", cancellationToken);

            _logger.LogInformation("QR Code généré pour le bon {Numero} par {User}",
                bon.NumeroReference, _currentUserService.GetUserLogin());

            return BonSortieResult.Ok(bon, "QR Code généré avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du QR Code pour le bon {Id}", bonId);
            return BonSortieResult.Fail($"Erreur lors de la génération du QR Code: {ex.Message}");
        }
    }

    public async Task<BonSortieQRValidationResult> ValidateQRCodeAsync(string scannedCode, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(scannedCode))
                return BonSortieQRValidationResult.Invalid("Code scanné vide");

            var validationResult = await _qrCodeService.ValidateQRCodeAsync(scannedCode);
            if (!validationResult.IsValid)
                return BonSortieQRValidationResult.Invalid(validationResult.ErrorMessage ?? "QR Code invalide");

            var bon = await _repository.GetByQRCodeHashAsync(scannedCode, cancellationToken);
            if (bon == null)
            {
                var decodedInfo = _qrCodeService.DecodeHash(scannedCode);
                if (decodedInfo != null && decodedInfo.BonId > 0)
                {
                    bon = await _repository.GetByIdAsync(decodedInfo.BonId, cancellationToken);
                }
            }

            if (bon == null)
                return BonSortieQRValidationResult.Invalid("Aucun bon de sortie associé à ce QR Code");

            if (bon.StatutActuel != "Approved")
                return BonSortieQRValidationResult.Invalid($"Le bon n'est pas approuvé (statut: {bon.StatutActuel})");

            var result = BonSortieQRValidationResult.Valid(bon);
            if (result.IsExpired)
            {
                result.Message = $"QR Code valide mais le bon a expiré le {bon.DateExpiration:dd/MM/yyyy}";
            }
            else
            {
                result.Message = $"Bon valide jusqu'au {bon.DateExpiration:dd/MM/yyyy}";
            }

            _logger.LogInformation("QR Code validé pour le bon {Numero} (expiré: {Expired})",
                bon.NumeroReference, result.IsExpired);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du QR Code");
            return BonSortieQRValidationResult.Invalid($"Erreur de validation: {ex.Message}");
        }
    }

    #endregion

    #region Liaison Entrée-Sortie (BSM-031)

    public async Task<BonEntreeBasicInfo?> GetBonEntreeAssocieAsync(int bonSortieId, CancellationToken cancellationToken = default)
    {
        var bon = await _repository.GetByIdAsync(bonSortieId, cancellationToken);

        if (bon is not BonSortieExterne externe || !externe.BonEntreeAssocieId.HasValue)
            return null;

        return await _bonEntreeLockService.GetBasicInfoAsync(externe.BonEntreeAssocieId.Value, cancellationToken);
    }

    public async Task<BonEntreeAvailabilityResult> CheckBonEntreeAvailabilityAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        return await _bonEntreeLockService.CheckAvailabilityAsync(bonEntreeId, cancellationToken);
    }

    public async Task<BonEntreeDetailsPourSortieDto?> GetBonEntreeDetailsForSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        var details = await _bonEntreeLockService.GetDetailsForSortieAsync(bonEntreeId, cancellationToken);
        if (details == null)
            return null;

        return new BonEntreeDetailsPourSortieDto
        {
            IdBon = details.IdBon,
            NumeroReference = details.NumeroReference,
            NomCompagnie = details.NomCompagnie,
            ContratId = details.ContratId,
            NumeroContrat = details.NumeroContrat,
            SiteManager = details.SiteManager,
            HostDepartment = details.HostDepartment,
            ReasonOnSite = details.ReasonOnSite,
            Provenance = details.Provenance,
            Destination = details.Destination,
            DateCreation = details.DateCreation,
            DateExpiration = details.DateExpiration,
            StatutActuel = details.StatutActuel,
            Materiels = details.Materiels.Select(m => new MaterielPourSortieDto
            {
                IdMateriel = m.IdMateriel,
                CodeProduitSerial = m.CodeProduitSerial,
                Designation = m.Designation,
                QuantiteInitiale = m.QuantiteInitiale,
                QuantiteDejaSortie = m.QuantiteDejaSortie
            }).ToList()
        };
    }

    public async Task<BonEntreeDetailsPourSortieDto?> SearchBonEntreeByReferenceAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        var details = await _bonEntreeLockService.SearchByNumeroReferenceAsync(numeroReference, cancellationToken);
        if (details == null)
            return null;

        return new BonEntreeDetailsPourSortieDto
        {
            IdBon = details.IdBon,
            NumeroReference = details.NumeroReference,
            NomCompagnie = details.NomCompagnie,
            ContratId = details.ContratId,
            NumeroContrat = details.NumeroContrat,
            SiteManager = details.SiteManager,
            HostDepartment = details.HostDepartment,
            ReasonOnSite = details.ReasonOnSite,
            Provenance = details.Provenance,
            Destination = details.Destination,
            DateCreation = details.DateCreation,
            DateExpiration = details.DateExpiration,
            StatutActuel = details.StatutActuel,
            Materiels = details.Materiels.Select(m => new MaterielPourSortieDto
            {
                IdMateriel = m.IdMateriel,
                CodeProduitSerial = m.CodeProduitSerial,
                Designation = m.Designation,
                QuantiteInitiale = m.QuantiteInitiale,
                QuantiteDejaSortie = m.QuantiteDejaSortie
            }).ToList()
        };
    }

    #endregion

    #region Stats

    public async Task<BonSortieStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var countByStatut = await _repository.GetCountByStatutAsync(cancellationToken);
        var activeLoans = await _repository.GetActiveLoansAsync(cancellationToken);
        var overdueLoans = await _repository.GetOverdueLoansAsync(cancellationToken);

        var pendingStatuts = new[] { "PendingIT", "PendingEnv", "PendingSuperviseur", "PendingGM", "PendingOPJ", "PendingIdentification" };

        return new BonSortieStats
        {
            TotalBons = countByStatut.Values.Sum(),
            BonsEnAttente = countByStatut.Where(x => pendingStatuts.Contains(x.Key)).Sum(x => x.Value),
            BonsApprouves = countByStatut.GetValueOrDefault("Approved", 0),
            BonsRejetes = countByStatut.GetValueOrDefault("Rejected", 0),
            PretsEnCours = activeLoans.Count,
            PretsEnRetard = overdueLoans.Count
        };
    }

    public async Task<IReadOnlyList<BonSortieLieInfo>> GetBonsSortieByBonEntreeAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        var bons = await _repository.GetByBonEntreeAsync(bonEntreeId, cancellationToken);

        return bons.Select(b => new BonSortieLieInfo
        {
            IdBonSortie = b.Id,
            NumeroReference = b.NumeroReference,
            TypeBon = b is Pret ? "Prêt" : "Externe",
            StatutActuel = b.StatutActuel,
            DateCreation = b.DateCreation,
            Destination = b.Destination,
            NomDemandeur = b.NomDemandeur,
            NombreMateriels = b.Materiels.Count
        }).ToList();
    }

    #endregion

    #region Private Methods

    private async Task AddHistoryAsync(int bonSortieId, ActionBonSortie action, string? statutAvant, string? statutApres, string description, CancellationToken cancellationToken)
    {
        var history = new BonSortieHistory(
            bonId: bonSortieId,
            action: action,
            actionDescription: description,
            actionBy: _currentUserService.GetUserLogin(),
            actionByNom: _currentUserService.GetUserDisplayName(),
            statutAvant: statutAvant,
            statutApres: statutApres);

        await _repository.AddHistoryAsync(history, cancellationToken);
    }

    #endregion
}
