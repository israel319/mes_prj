using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.BonEntree;

/// <summary>
/// Service métier pour les Bons d'Entrée.
/// Simplifié selon le diagramme de classe.
/// </summary>
public class BonEntreeService : IBonEntreeService
{
    private readonly IBonEntreeRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IQRCodeService _qrCodeService;
    private readonly IBarriereService _barriereService;
    private readonly INotificationRejetService _notificationRejetService;
    private readonly IWorkflowConfigService _workflowConfigService;
    private readonly IChaineApprobationService _chaineApprobationService;
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<BonEntreeService> _logger;

    private const int DefaultValidityDays = 7;

    public BonEntreeService(
        IBonEntreeRepository repository,
        ICurrentUserService currentUserService,
        IQRCodeService qrCodeService,
        IBarriereService barriereService,
        INotificationRejetService notificationRejetService,
        IWorkflowConfigService workflowConfigService,
        IChaineApprobationService chaineApprobationService,
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<BonEntreeService> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _qrCodeService = qrCodeService;
        _barriereService = barriereService;
        _notificationRejetService = notificationRejetService;
        _workflowConfigService = workflowConfigService;
        _chaineApprobationService = chaineApprobationService;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    private string CurrentLogin => _currentUserService.GetUserLogin();
    private string CurrentDisplayName => _currentUserService.GetUserDisplayName();

    #region CRUD Operations

    public async Task<BonEntreeResult> CreateAsync(CreateBonEntreeRequest request, CancellationToken cancellationToken = default)
    {
        var validationErrors = ValidateCreateRequest(request);
        if (validationErrors.Count > 0)
        {
            return BonEntreeResult.Fail(validationErrors);
        }

        var createResult = Domain.Entities.BonEntree.Create(
            nomDemandeur: _currentUserService.GetUserDisplayName(),
            nomCompagnie: request.NomCompagnie,
            siteManager: request.SiteManager,
            hostDepartment: request.HostDepartment,
            reasonOnSite: request.ReasonOnSite,
            nomEscorteur: request.NomEscorteur,
            provenance: request.Provenance,
            destination: request.Destination,
            dateExpiration: request.DateExpiration != default
                ? request.DateExpiration
                : DateTime.Now.AddDays(DefaultValidityDays),
            description: request.Description,
            emailContractant: request.EmailContractant,
            fonctionEscorteur: request.FonctionEscorteur,
            contratId: request.ContratId,
            numeroContrat: request.NumeroContrat);

        if (createResult.IsFailure)
        {
            return BonEntreeResult.Fail(createResult.Error.Message);
        }

        var bonEntree = createResult.Value;
        bonEntree.DepartementId = request.DepartementId;
        bonEntree.RaisonEntreeId = request.RaisonEntreeId;

        // v2 — Site KCC + RequestedFor (chaîne d'approbation par site/département)
        bonEntree.SiteId = request.SiteId;
        bonEntree.RequestedForEmployeeCode = request.RequestedForEmployeeCode;
        bonEntree.RequestedForDisplay = request.RequestedForDisplay;
        bonEntree.RequestedForDepartement = request.RequestedForDepartement;

        // Ajouter les matériels via la méthode Domain
        foreach (var mat in request.Materiels)
        {
            var addResult = bonEntree.AjouterMateriel(mat.CodeProduitSerial, mat.Designation, mat.Quantite);
            if (addResult.IsFailure)
            {
                return BonEntreeResult.Fail(addResult.Error.Message);
            }
        }

        var created = await _repository.CreateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(created.Id, ActionBonEntree.Creation, null, "Draft",
            $"Nouveau bon {created.NumeroReference}", null, cancellationToken);

        _logger.LogInformation("Bon d'entrée {Numero} créé par {User}", created.NumeroReference, CurrentLogin);

        return BonEntreeResult.Ok(created, $"Bon d'entrée {created.NumeroReference} créé avec succès");
    }

    public async Task<BonEntreeResult> UpdateAsync(UpdateBonEntreeRequest request, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(request.IdBon, includeMateriels: true, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        if (bonEntree.StatutActuel != "Draft")
        {
            return BonEntreeResult.Fail($"Le bon ne peut pas être modifié dans le statut {bonEntree.StatutActuel}");
        }

        bonEntree.ContratId = request.ContratId;
        bonEntree.NumeroContrat = request.NumeroContrat;
        bonEntree.NomCompagnie = request.NomCompagnie;
        bonEntree.EmailContractant = request.EmailContractant;
        bonEntree.SiteManager = request.SiteManager;
        bonEntree.HostDepartment = request.HostDepartment;
        bonEntree.DepartementId = request.DepartementId;
        bonEntree.RaisonEntreeId = request.RaisonEntreeId;
        bonEntree.ReasonOnSite = request.ReasonOnSite;
        bonEntree.NomEscorteur = request.NomEscorteur;
        bonEntree.FonctionEscorteur = request.FonctionEscorteur;
        bonEntree.Provenance = request.Provenance;
        bonEntree.Destination = request.Destination;
        bonEntree.DateExpiration = request.DateExpiration;
        bonEntree.Description = request.Description;

        // v2 — Site KCC + RequestedFor
        bonEntree.SiteId = request.SiteId;
        bonEntree.RequestedForEmployeeCode = request.RequestedForEmployeeCode;
        bonEntree.RequestedForDisplay = request.RequestedForDisplay;
        bonEntree.RequestedForDepartement = request.RequestedForDepartement;

        // Mise à jour des matériels (uniquement en brouillon)
        if (request.Materiels != null && request.Materiels.Any())
        {
            // Supprimer les matériels qui ne sont plus dans la liste
            var requestMaterielIds = request.Materiels.Where(m => m.IdMateriel > 0).Select(m => m.IdMateriel).ToList();
            var materielsToRemove = bonEntree.Materiels.Where(m => !requestMaterielIds.Contains(m.Id)).ToList();
            foreach (var mat in materielsToRemove)
            {
                bonEntree.SupprimerMateriel(mat.Id);
            }

            // Mettre à jour ou ajouter les matériels
            foreach (var matRequest in request.Materiels)
            {
                if (matRequest.IdMateriel > 0)
                {
                    // Mise à jour d'un matériel existant — repo gère directement
                    var existingMat = bonEntree.Materiels.FirstOrDefault(m => m.Id == matRequest.IdMateriel);
                    if (existingMat != null)
                    {
                        existingMat.CodeProduitSerial = matRequest.CodeProduitSerial;
                        existingMat.Designation = matRequest.Designation;
                        existingMat.Quantite = matRequest.Quantite;
                        existingMat.QuantiteDisponible = matRequest.Quantite; // En brouillon, disponible = quantité
                    }
                }
                else
                {
                    // Nouveau matériel via méthode Domain
                    bonEntree.AjouterMateriel(matRequest.CodeProduitSerial, matRequest.Designation, matRequest.Quantite);
                }
            }
        }
        else
        {
            // Si aucun matériel fourni, supprimer tous les matériels existants
            foreach (var mat in bonEntree.Materiels.ToList())
            {
                bonEntree.SupprimerMateriel(mat.Id);
            }
        }

        var updated = await _repository.UpdateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(updated.Id, ActionBonEntree.Modification, "Draft", "Draft",
            $"Bon modifié", null, cancellationToken);

        _logger.LogInformation("Bon d'entrée {Numero} modifié par {User}", updated.NumeroReference, CurrentLogin);

        return BonEntreeResult.Ok(updated, "Bon d'entrée mis à jour avec succès");
    }

    public async Task<Domain.Entities.BonEntree?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id,
            includeMateriels: true,
            includeApprobations: true,
            cancellationToken: cancellationToken);
    }

    public async Task<Domain.Entities.BonEntree?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByNumeroAsync(numeroReference, cancellationToken);
    }

    public async Task<BonEntreeSearchResult> GetListAsync(BonEntreeFilter filter, CancellationToken cancellationToken = default)
    {
        return await _repository.SearchAsync(filter, cancellationToken);
    }

    public async Task<BonEntreeResult> CancelAsync(int id, string motif, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(id, includeMateriels: false, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        var oldStatutCancel = bonEntree.StatutActuel;
        await _repository.DeleteAsync(id, motif, cancellationToken);

        await AddHistoryAsync(id, ActionBonEntree.Annulation, oldStatutCancel, "Cancelled",
            $"Annulé: {motif}", motif, cancellationToken);

        _logger.LogInformation("Bon d'entrée {Id} annulé par {User}: {Motif}", id, CurrentLogin, motif);

        return BonEntreeResult.Ok(bonEntree, "Bon d'entrée annulé avec succès");
    }

    public async Task<BonEntreeResult> DeleteDraftAsync(int id, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(id, includeMateriels: false, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        if (bonEntree.StatutActuel != "Draft")
        {
            return BonEntreeResult.Fail("Seuls les brouillons peuvent être supprimés");
        }

        // Vérifier que l'utilisateur est le créateur ou admin
        var currentUser = _currentUserService.GetUserDisplayName();
        var isAdmin = _currentUserService.IsInAnyRole("Admin");

        if (bonEntree.NomDemandeur != currentUser && !isAdmin)
        {
            return BonEntreeResult.Fail("Vous n'êtes pas autorisé à supprimer ce brouillon");
        }

        await _repository.DeleteAsync(id, "Brouillon supprimé par l'utilisateur", cancellationToken);

        _logger.LogInformation("Brouillon BEM {Id} ({Numero}) supprimé par {User}",
            id, bonEntree.NumeroReference, CurrentLogin);

        return BonEntreeResult.Ok(bonEntree, $"Le brouillon {bonEntree.NumeroReference} a été supprimé");
    }

    #endregion

    #region User Specific

    public async Task<BonEntreeSearchResult> GetMyBonsAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByCreateurAsync(CurrentLogin, skip, take, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.BonEntree>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
    {
        var roles = _currentUserService.GetUserRoles();
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

    public async Task<IReadOnlyList<ReturnedBemInfo>> GetMyReturnedBonsAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = _currentUserService.GetUserDisplayName();

        // Récupérer tous les bons de l'utilisateur en état Draft
        var result = await _repository.GetByCreateurAsync(currentUser, 0, 100, cancellationToken);

        var returnedBons = new List<ReturnedBemInfo>();

        foreach (var bon in result.Items.Where(b => b.StatutActuel == "Draft"))
        {
            // Vérifier s'il y a un historique de retour
            var dernierRetour = bon.Historiques
                .Where(h => h.Action == ActionBonEntree.RetourModification)
                .OrderByDescending(h => h.ActionDate)
                .FirstOrDefault();

            if (dernierRetour != null)
            {
                returnedBons.Add(new ReturnedBemInfo
                {
                    IdBon = bon.Id,
                    NumeroReference = bon.NumeroReference,
                    RaisonRetour = dernierRetour.Comment ?? dernierRetour.ActionDescription ?? "",
                    DateRetour = dernierRetour.ActionDate,
                    AuteurRetour = dernierRetour.ActionByNom ?? dernierRetour.ActionBy
                });
            }
        }

        return returnedBons.OrderByDescending(r => r.DateRetour).ToList();
    }

    public async Task<IReadOnlyList<Domain.Entities.BonEntree>> GetApprovedByMeAsync(CancellationToken cancellationToken = default)
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
            // L'approbateur courant doit avoir la dernière approbation (OrdreEtape max) avec decision Approuvé
            var lastApproved = b.Approbations
                .Where(a => a.Decision is "Approuvé" or "Approuve")
                .OrderByDescending(a => a.OrdreEtape)
                .FirstOrDefault();

            if (lastApproved == null) return false;

            // Stratégie 1 : EmployeeId
            if (lastApproved.ApprobateurId.HasValue && empId.HasValue && lastApproved.ApprobateurId == empId)
                return true;

            // Stratégie 2 : login
            var approvalLoginNorm = NormLogin(lastApproved.ApprobateurLogin);
            var currentLoginNorm  = NormLogin(currentUser);
            if (!string.IsNullOrEmpty(approvalLoginNorm) && !string.IsNullOrEmpty(currentLoginNorm)
                && approvalLoginNorm == currentLoginNorm)
                return true;

            // Stratégie 3 : matricule
            var approvalMat = lastApproved.ApprobateurMatricule?.Trim().ToUpperInvariant() ?? "";
            var currentMat  = currentUserMatricule.Trim().ToUpperInvariant();
            return !string.IsNullOrEmpty(approvalMat) && !string.IsNullOrEmpty(currentMat)
                   && approvalMat == currentMat;
        }).ToList().AsReadOnly();
    }

    #endregion

    #region Workflow

    public async Task<BonEntreeResult> SubmitForApprovalAsync(int id, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        if (bonEntree.StatutActuel != "Draft")
        {
            return BonEntreeResult.Fail("Seuls les bons en brouillon peuvent être soumis");
        }

        if (!bonEntree.Materiels.Any())
        {
            return BonEntreeResult.Fail("Le bon doit contenir au moins un matériel");
        }

        var oldStatut = bonEntree.StatutActuel;
        bonEntree.StatutActuel = "PendingSup";
        await _repository.UpdateAsync(bonEntree, cancellationToken);

        // Supprimer les éventuelles approbations existantes (données pré-migration ou soumission précédente)
        await _repository.DeleteApprobationsAsync(bonEntree.Id, cancellationToken);
        await CreateApprovalStepsAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(bonEntree.Id, ActionBonEntree.Soumission, oldStatut, "PendingSup",
            $"Envoyé pour validation", null, cancellationToken);

        _logger.LogInformation("Bon {Numero} soumis pour approbation par {User}", bonEntree.NumeroReference, CurrentLogin);

        return BonEntreeResult.Ok(bonEntree, "Bon soumis pour approbation");
    }

    public async Task<BonEntreeResult> ApproveAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(request.BonId, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        var approbations = await _repository.GetApprobationsAsync(request.BonId, cancellationToken);
        var currentStep = approbations
            .Where(a => a.Decision == "En attente")
            .OrderBy(a => a.OrdreEtape)
            .FirstOrDefault();

        if (currentStep == null)
        {
            return BonEntreeResult.Fail("Aucune approbation en attente pour ce bon");
        }

        // Résoudre l'identité de l'utilisateur courant avant le refresh (nécessaire pour la sélection multi-approbateurs).
        var currentEmpId = await ResolveCurrentEmployeeIdAsync(cancellationToken);

        // Refresh : synchroniser l'approbateur depuis la config actuelle — si plusieurs approbateurs actifs pour l'étape,
        // le candidat courant est prioritaire (premier qui approuve fait le job).
        await RefreshCurrentStepApproverAsync(bonEntree, currentStep, cancellationToken, currentEmpId);

        // Auth v2 : vérifier que l'utilisateur courant est bien l'approbateur désigné (ou un co-approbateur OR pour Identification),
        // ou un Admin niveau >= L2 (override).
        var isAdminOverride = _currentUserService.GetUserRoles().Any(r =>
            string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

        if (!isAdminOverride && (currentEmpId == null || currentStep.ApprobateurId != currentEmpId))
        {
            return BonEntreeResult.Fail(
                $"Vous n'êtes pas autorisé à approuver cette étape. Approbateur attendu : {currentStep.NomApprobateur ?? currentStep.NomEtape}.");
        }

        // Enregistrer les infos de l'approbateur via méthode Domain
        var oldStatutApprove = bonEntree.StatutActuel;
        currentStep.Approuver(
            _currentUserService.GetUserDisplayName(),
            currentStep.RoleApprobateur ?? currentStep.NomEtape ?? "",
            request.ReservesEventuelles);

        await _repository.UpsertApprobationAsync(currentStep, cancellationToken);

        var nextStep = approbations
            .Where(a => a.OrdreEtape > currentStep.OrdreEtape && a.Decision == "En attente")
            .OrderBy(a => a.OrdreEtape)
            .FirstOrDefault();

        if (nextStep != null)
        {
            bonEntree.StatutActuel = GetStatutForCodeEtape(nextStep.CodeEtape);
            // Synchroniser le pointeur "prochain approbateur".
            if (nextStep.ApprobateurId.HasValue)
            {
                bonEntree.AssignerProchainApprobateur(nextStep.ApprobateurId.Value, nextStep.NomApprobateur ?? string.Empty, nextStep.OrdreEtape);
            }
        }
        else
        {
            // Approbation finale - Génération automatique du QR Code
            bonEntree.StatutActuel = "Approved";

            try
            {
                var qrResult = _qrCodeService.GenerateQRCode(bonEntree.Id, "BEM", bonEntree.NumeroReference);
                bonEntree.SetQRCode(
                    $"{{\"IdBon\":{bonEntree.Id},\"Type\":\"BEM\",\"Reference\":\"{bonEntree.NumeroReference}\"}}",
                    qrResult.QRCodeBase64,
                    qrResult.HashedCode);

                _logger.LogInformation("QR Code généré automatiquement pour le bon approuvé {Numero}", bonEntree.NumeroReference);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Échec de la génération du QR Code pour le bon {Numero}", bonEntree.NumeroReference);
                // Ne pas bloquer l'approbation en cas d'erreur de génération QR
            }
        }

        await _repository.UpdateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(bonEntree.Id, ActionBonEntree.Approbation, oldStatutApprove, bonEntree.StatutActuel,
            $"Validé par {_currentUserService.GetUserDisplayName()} ({currentStep.NomEtape})",
            request.ReservesEventuelles, cancellationToken);

        _logger.LogInformation("Bon {Numero} approuvé par {User}", bonEntree.NumeroReference, CurrentLogin);

        return BonEntreeResult.Ok(bonEntree, "Approbation enregistrée");
    }

    public async Task<BonEntreeResult> RejectAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(request.BonId, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        var approbations = await _repository.GetApprobationsAsync(request.BonId, cancellationToken);
        var currentStep = approbations.FirstOrDefault(a => a.Decision == "En attente");

        if (currentStep == null)
        {
            return BonEntreeResult.Fail("Aucune approbation en attente pour ce bon");
        }

        // Résoudre l'identité de l'utilisateur courant avant le refresh (nécessaire pour la sélection multi-approbateurs).
        var currentEmpId = await ResolveCurrentEmployeeIdAsync(cancellationToken);

        // Refresh : synchroniser l'approbateur depuis la config actuelle — si plusieurs approbateurs actifs pour l'étape,
        // le candidat courant est prioritaire (premier qui rejette fait le job).
        await RefreshCurrentStepApproverAsync(bonEntree, currentStep, cancellationToken, currentEmpId);

        // Auth v2 : vérifier l'approbateur désigné (ou Admin override).
        var isAdminOverride = _currentUserService.GetUserRoles().Any(r =>
            string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(r, "SuperAdmin", StringComparison.OrdinalIgnoreCase));

        if (!isAdminOverride && (currentEmpId == null || currentStep.ApprobateurId != currentEmpId))
        {
            return BonEntreeResult.Fail(
                $"Vous n'êtes pas autorisé à rejeter cette étape. Approbateur attendu : {currentStep.NomApprobateur ?? currentStep.NomEtape}.");
        }

        // Enregistrer le rejet via méthode Domain
        currentStep.Rejeter(
            _currentUserService.GetUserDisplayName(),
            currentStep.RoleApprobateur ?? currentStep.NomEtape ?? "",
            request.ReservesEventuelles ?? "Aucun motif");

        await _repository.UpsertApprobationAsync(currentStep, cancellationToken);

        var oldStatutReject = bonEntree.StatutActuel;
        bonEntree.StatutActuel = "Rejected";
        await _repository.UpdateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(bonEntree.Id, ActionBonEntree.Rejet, oldStatutReject, "Rejected",
            $"Refusé par {_currentUserService.GetUserDisplayName()} ({currentStep.NomEtape})",
            request.ReservesEventuelles, cancellationToken);

        // Enregistrer la notification de rejet en base de données
        try
        {
            await _notificationRejetService.EnregistrerRejetAsync(
                bonType: "BEM",
                numeroReference: bonEntree.NumeroReference,
                etapeRejet: currentStep.NomEtape ?? "N/A",
                approbateurNom: _currentUserService.GetUserDisplayName(),
                approbateurLogin: CurrentLogin,
                motifRejet: request.ReservesEventuelles ?? "Aucun motif spécifié",
                demandeurNom: bonEntree.NomDemandeur,
                cancellationToken);
        }
        catch (Exception notifEx)
        {
            _logger.LogWarning(notifEx, "Échec de l'enregistrement de la notification de rejet pour BEM {Numero}", bonEntree.NumeroReference);
        }

        _logger.LogInformation("Bon {Numero} rejeté par {User}", bonEntree.NumeroReference, CurrentLogin);

        return BonEntreeResult.Ok(bonEntree, "Bon rejeté");
    }

    public async Task<BonEntreeResult> ReturnForModificationAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(request.BonId, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        var oldStatutReturn = bonEntree.StatutActuel;
        bonEntree.StatutActuel = "Draft";
        await _repository.UpdateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(bonEntree.Id, ActionBonEntree.RetourModification, oldStatutReturn, "Draft",
            $"Renvoyé par {_currentUserService.GetUserDisplayName()}",
            request.ReservesEventuelles, cancellationToken);

        _logger.LogInformation("Bon {Numero} retourné pour modification", bonEntree.NumeroReference);

        return BonEntreeResult.Ok(bonEntree, "Bon retourné pour modification");
    }

    #endregion

    #region QR Code & Itineraire

    public async Task<BonEntreeQRCodeResult> GenerateQRCodeAsync(int id, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeQRCodeResult.Fail("Bon d'entrée non trouvé");
        }

        if (bonEntree.StatutActuel != "Approved")
        {
            return BonEntreeQRCodeResult.Fail("Le bon doit être approuvé pour générer un QR Code");
        }

        var qrResult = _qrCodeService.GenerateQRCode(bonEntree.Id, "BEM", bonEntree.NumeroReference);

        _logger.LogInformation("QR Code généré pour le bon {Numero}", bonEntree.NumeroReference);

        return BonEntreeQRCodeResult.Ok(qrResult.QRCodeBase64, qrResult.HashedCode);
    }

    public async Task<IReadOnlyList<ItinerairePrevu>> CalculateItineraireAsync(int id, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(id, includeMateriels: false, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return [];
        }

        var barrieres = await _barriereService.GetAllAsync(cancellationToken);
        var itineraires = new List<ItinerairePrevu>();
        var ordre = 1;

        foreach (var barriere in barrieres.Take(2))
        {
            itineraires.Add(new ItinerairePrevu(id, ordre++, barriere.Id));
        }

        return itineraires;
    }

    #endregion

    #region Materiels

    public async Task<BonEntreeResult> AddMaterielAsync(int bonId, Materiel materiel, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _repository.GetByIdAsync(bonId, includeMateriels: false, cancellationToken: cancellationToken);
        if (bonEntree == null)
        {
            return BonEntreeResult.Fail("Bon d'entrée non trouvé");
        }

        if (bonEntree.StatutActuel != "Draft")
        {
            return BonEntreeResult.Fail("Le bon ne peut pas être modifié dans son statut actuel");
        }

        await _repository.AddMaterielAsync(bonId, materiel, cancellationToken);

        var updated = await _repository.GetByIdAsync(bonId, cancellationToken: cancellationToken);
        return BonEntreeResult.Ok(updated!, "Matériel ajouté");
    }

    public async Task<BonEntreeResult> UpdateMaterielAsync(Materiel materiel, CancellationToken cancellationToken = default)
    {
        await _repository.UpdateMaterielAsync(materiel, cancellationToken);
        return new BonEntreeResult { Success = true, Message = "Matériel mis à jour" };
    }

    public async Task<BonEntreeResult> DeleteMaterielAsync(int materielId, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteMaterielAsync(materielId, cancellationToken);
        return new BonEntreeResult { Success = true, Message = "Matériel supprimé" };
    }

    #endregion

    #region Statistics

    public async Task<BonEntreeStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var countByStatut = await _repository.GetCountByStatutAsync(cancellationToken);
        var todayCount = await _repository.GetTodayCountAsync(cancellationToken);

        return new BonEntreeStats
        {
            TotalBons = countByStatut.Values.Sum(),
            BonsAujourdHui = todayCount,
            BonsEnAttente = countByStatut
                .Where(kvp => kvp.Key.StartsWith("Pending"))
                .Sum(kvp => kvp.Value),
            BonsApprouves = countByStatut.GetValueOrDefault("Approved", 0),
            BonsEnTransit = countByStatut.GetValueOrDefault("InTransit", 0),
            BonsCompletes = countByStatut.GetValueOrDefault("Completed", 0),
            ParStatut = countByStatut
        };
    }

    #endregion

    #region Approbations

    public async Task<IReadOnlyList<Approbation>> GetApprobationsAsync(int bonId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetApprobationsAsync(bonId, cancellationToken);
    }

    #endregion

    #region Private Helpers

    private async Task AddHistoryAsync(int bonId, ActionBonEntree action, string? statutAvant, string? statutApres,
        string description, string? comment, CancellationToken cancellationToken)
    {
        var history = new BonEntreeHistory(
            bonId: bonId,
            action: action,
            actionDescription: description,
            actionBy: _currentUserService.GetUserLogin(),
            actionByNom: _currentUserService.GetUserDisplayName(),
            comment: comment,
            statutAvant: statutAvant,
            statutApres: statutApres);

        await _repository.AddHistoryAsync(history, cancellationToken);
    }

    private static List<string> ValidateCreateRequest(CreateBonEntreeRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.NomCompagnie))
            errors.Add("Le nom de la compagnie est obligatoire");

        if (string.IsNullOrWhiteSpace(request.SiteManager))
            errors.Add("Le Site Manager est obligatoire");

        if (string.IsNullOrWhiteSpace(request.HostDepartment))
            errors.Add("Le département hôte est obligatoire");

        if (string.IsNullOrWhiteSpace(request.ReasonOnSite))
            errors.Add("Le motif de la visite est obligatoire");

        if (string.IsNullOrWhiteSpace(request.Provenance))
            errors.Add("La provenance est obligatoire");

        if (string.IsNullOrWhiteSpace(request.Destination))
            errors.Add("La destination est obligatoire");

        if (string.IsNullOrWhiteSpace(request.NomEscorteur))
            errors.Add("Le nom de l'escorteur est obligatoire");

        return errors;
    }

    private async Task CreateApprovalStepsAsync(Domain.Entities.BonEntree bon, CancellationToken cancellationToken)
    {
        // Chaîne v2 — fondée sur le référentiel Glencore (ReportsTo → GM → [IT|Env] → OPJ → Identification).
        // L'employé créateur est résolu via Login (CreatedBy = login Windows).
        var creatorLogin = !string.IsNullOrWhiteSpace(bon.CreatedBy)
            ? bon.CreatedBy
            : _currentUserService.GetUserLogin();

        if (string.IsNullOrWhiteSpace(creatorLogin))
        {
            _logger.LogError("Impossible de construire la chaîne d'approbation : CreatedBy et login courant sont vides (bon {BonId}).", bon.Id);
            throw new InvalidOperationException(
                "Impossible de déterminer le créateur du bon : CreatedBy et login courant sont vides.");
        }
        var creator = await ResolveOrProvisionEmployeeAsync(creatorLogin, cancellationToken);
        if (creator == null)
        {
            _logger.LogError(
                "Impossible de construire la chaîne d'approbation : aucun Employee local pour CreatedBy='{Login}' (bon {BonId}).",
                creatorLogin, bon.Id);
            throw new InvalidOperationException(
                $"Aucun Employee local trouvé pour '{creatorLogin}' (login ou matricule). " +
                "Vérifier l'auto-création depuis Glencore.");
        }

        // BonEntree : pas de TypeMateriel typé (un bon peut contenir plusieurs types) → pas de routage IT/Env.
        var chain = await _chaineApprobationService.ConstruireChaineAsync(
            creator.Id, descriptionMateriel: null, siteId: bon.SiteId, ct: cancellationToken);

        if (chain.Etapes.Count == 0)
            throw new InvalidOperationException($"Chaîne d'approbation vide pour le bon {bon.NumeroReference}.");

        foreach (var etape in chain.Etapes.OrderBy(e => e.Ordre))
        {
            var step = new Approbation(
                bonId: bon.Id,
                ordreEtape: etape.Ordre,
                codeEtape: etape.Kind.ToString().ToUpperInvariant(),
                nomEtape: GetEtapeLabel(etape.Kind),
                approbateurId: etape.EmployeeId,
                approbateurMatricule: etape.EmployeeMatricule,
                approbateurLogin: etape.EmployeeLogin,
                nomApprobateur: etape.EmployeeNomComplet);
            await _repository.UpsertApprobationAsync(step, cancellationToken);
        }

        // Synchroniser le pointeur "prochain approbateur" sur le bon.
        var first = chain.Etapes.OrderBy(e => e.Ordre).First();
        bon.AssignerProchainApprobateur(first.EmployeeId, first.EmployeeNomComplet, first.Ordre);
        await _repository.UpdateAsync(bon, cancellationToken);
    }

    /// <summary>Libellé français standard d'une étape (kind → label).</summary>
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

    /// <summary>
    /// Mapping CodeEtape (kind stable) → valeur StatutActuel.
    /// </summary>
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

    /// <summary>
    /// Retourne le rôle requis pour une étape d'approbation donnée (hérité — n’est plus utilisé par Approve/Reject).
    /// </summary>
    private string GetRequiredRoleForStep(string? nomEtape)
    {
        return nomEtape?.ToLowerInvariant() switch
        {
            "superviseur" => "Superviseur",
            "general manager" => "GM",
            "opj" => "OPJ",
            "identification" => "Identification",
            _ => nomEtape ?? ""
        };
    }

    private static string GetStatutForEtape(int ordreEtape)
    {
        return ordreEtape switch
        {
            1 => "PendingSup",           // Superviseur
            2 => "PendingGM",            // General Manager
            3 => "PendingOPJ",           // OPJ
            4 => "PendingIdentification", // Identification (étape finale)
            _ => "PendingSup"
        };
    }

    /// <summary>
    /// Synchronise l'approbateur d'une étape en attente depuis la configuration actuelle.
    /// — Étapes spéciales (OPJ, Identification) : requête directe dans WorkflowApprobateursSpeciaux,
    ///   indépendamment du reste de la chaîne (résistant à la suppression/remplacement de l'approbateur).
    /// — Étapes Glencore (SuperIntendent, GM, ReportsTo) : reconstruction de la chaîne.
    /// </summary>
    private async Task RefreshCurrentStepApproverAsync(
        Domain.Entities.BonEntree bon, Approbation step, CancellationToken ct, int? candidateEmpId = null)
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
                var creatorLogin = !string.IsNullOrWhiteSpace(bon.CreatedBy) ? bon.CreatedBy : null;
                if (string.IsNullOrWhiteSpace(creatorLogin)) return;

                var creator = await ResolveOrProvisionEmployeeAsync(creatorLogin, ct);
                if (creator == null) return;

                var freshChain = await _chaineApprobationService.ConstruireChaineAsync(
                    creator.Id, descriptionMateriel: null, siteId: bon.SiteId, ct: ct);

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
                await _repository.UpsertApprobationAsync(step, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Impossible de rafraîchir l'approbateur pour l'étape {Code} du bon {Ref}",
                codeUpper, bon.NumeroReference);
        }
    }

    #endregion
}
