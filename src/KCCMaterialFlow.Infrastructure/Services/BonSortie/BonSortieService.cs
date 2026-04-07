using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonSortie.DTOs;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
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

            // Auto-dériver TypeMateriel depuis RaisonSortie (source unique de vérité)
            var typeMateriel = request.TypeMateriel;
            var resolvedRaisonCode = raisonCode ?? request.RaisonSortieCode;
            if (!string.IsNullOrEmpty(resolvedRaisonCode))
            {
                var resolved = await _repository.GetTypeMaterielByRaisonCodeAsync(resolvedRaisonCode, cancellationToken);
                if (resolved.HasValue)
                {
                    typeMateriel = resolved.Value;
                    _logger.LogInformation("TypeMateriel auto-dérivé depuis RaisonSortie '{Code}': {Type}", resolvedRaisonCode, typeMateriel);
                }
            }

            // BSM-025: Logique Circulaire - pas de lien BonEntree, validité 6 mois
            // BSM-020: Pour les autres types, BonEntree est obligatoire
            if (request.TypeMateriel == TypeMateriel.Circulaire || typeMateriel == TypeMateriel.Circulaire)
            {
                var maxExpiration = DateTime.Now.AddMonths(6);
                if (request.DateExpiration > maxExpiration)
                {
                    return BonSortieResult.Fail($"Pour un bon circulaire, la date d'expiration ne peut pas dépasser 6 mois ({maxExpiration:dd/MM/yyyy})");
                }
            }
            else
            {
                // BSM-020 & BSM-031: Pour les autres types, BonEntree associé obligatoire
                if (!request.BonEntreeAssocieId.HasValue)
                {
                    return BonSortieResult.Fail("Un bon d'entrée associé est obligatoire pour ce type de sortie (sauf Circulaire)");
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
                    typeMateriel,
                    cancellationToken);
                if (!string.IsNullOrWhiteSpace(coherenceError))
                {
                    return BonSortieResult.Fail(coherenceError);
                }
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
                typeMateriel: typeMateriel,
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

            var typeMateriel = request.TypeMateriel;
            var resolvedRaisonCode = raisonCode ?? request.RaisonSortieCode;
            if (!string.IsNullOrEmpty(resolvedRaisonCode))
            {
                var resolved = await _repository.GetTypeMaterielByRaisonCodeAsync(resolvedRaisonCode, cancellationToken);
                if (resolved.HasValue)
                {
                    typeMateriel = resolved.Value;
                    _logger.LogInformation("TypeMateriel auto-dérivé depuis RaisonSortie '{Code}': {Type}", resolvedRaisonCode, typeMateriel);
                }
            }

            if (request.BonEntreeAssocieId.HasValue)
            {
                var coherenceError = await ValidateBonEntreeMotifCoherenceAsync(
                    request.BonEntreeAssocieId.Value,
                    resolvedRaisonCode,
                    typeMateriel,
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
                typeMateriel: typeMateriel,
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

            var typeMateriel = request.TypeMateriel;
            var resolvedRaisonCode = raisonCode ?? request.RaisonSortieCode;
            if (!string.IsNullOrEmpty(resolvedRaisonCode))
            {
                var resolved = await _repository.GetTypeMaterielByRaisonCodeAsync(resolvedRaisonCode, cancellationToken);
                if (resolved.HasValue)
                {
                    typeMateriel = resolved.Value;
                    _logger.LogInformation("TypeMateriel auto-dérivé depuis RaisonSortie '{Code}': {Type}", resolvedRaisonCode, typeMateriel);
                }
            }

            if (request.BonEntreeAssocieId.HasValue)
            {
                var coherenceError = await ValidateBonEntreeMotifCoherenceAsync(
                    request.BonEntreeAssocieId.Value,
                    resolvedRaisonCode,
                    typeMateriel,
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
                typeMateriel: typeMateriel,
                dateRetourPrevue: dateRetourPrevue,
                bonEntreeAssocieId: request.BonEntreeAssocieId,
                raisonSortieCode: resolvedRaisonCode,
                description: request.Description,
                adresseDestination: request.AdresseDestination);

            if (createResult.IsFailure)
                return BonSortieResult.Fail(createResult.Error.Message);

            var pret = createResult.Value;
            pret.SetNumeroReference(numeroReference);

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

    private async Task<string?> ValidateBonEntreeMotifCoherenceAsync(int bonEntreeId, string? raisonSortieCode, TypeMateriel typeMateriel, CancellationToken cancellationToken)
    {
        // Résoudre le département attendu depuis la BD (remplace le mapping hardcodé)
        var expectedDeptCode = !string.IsNullOrWhiteSpace(raisonSortieCode)
            ? await _categorieSortieService.GetDepartementCodeForRaisonAsync(raisonSortieCode)
            : null;

        if (string.IsNullOrWhiteSpace(expectedDeptCode))
        {
            return null; // Raison fait partie du mapping par défaut, pas de contrainte de département
        }

        var bemDetails = await _bonEntreeLockService.GetDetailsForSortieAsync(bonEntreeId, cancellationToken);
        if (bemDetails == null)
        {
            return "Bon d'entrée associé introuvable pour contrôle de cohérence métier.";
        }

        var hostDepartment = bemDetails.HostDepartment?.Trim();
        if (string.IsNullOrWhiteSpace(hostDepartment))
        {
            return $"Le BEM {bemDetails.NumeroReference} n'a pas de département hôte renseigné. Impossible de valider la cohérence avec le motif sélectionné.";
        }

        if (!hostDepartment.Contains(expectedDeptCode, StringComparison.OrdinalIgnoreCase))
        {
            return $"Incohérence métier: le motif '{raisonSortieCode ?? typeMateriel.ToString()}' requiert un BEM du département '{expectedDeptCode}', mais le BEM sélectionné est rattaché à '{hostDepartment}'.";
        }

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
        var roles = _currentUserService.GetUserRoles();
        var allPending = new List<Domain.Entities.BonSortie>();

        foreach (var role in roles)
        {
            var pending = await _repository.GetPendingApprovalAsync(role, cancellationToken);
            allPending.AddRange(pending);
        }

        return allPending.DistinctBy(b => b.Id).ToList();
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
            // NomEtape stocke le RoleCode canonisé — c'est la source de vérité pour le routing
            var newStatut = GetPendingStatusForRoleCode(firstStep.NomEtape);
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
        var workflowEtapes = await _workflowConfigService.GetResolvedWorkflowEtapesAsync("BSM", bon.RaisonSortieCode, cancellationToken);

        if (workflowEtapes.Count == 0)
        {
            workflowEtapes = BuildDefaultWorkflowEtapes(bon).ToList();
        }

        var ordreEtape = 1;
        foreach (var etape in workflowEtapes.OrderBy(x => x.OrdreEtape))
        {
            var approbation = new ApprobationSortie(bon.Id, ordreEtape++, CanonicalizeRoleCode(etape.RoleCode));
            // ApprobationSortie a public set — on peut aussi stocker le nom lisible dans RoleApprobateur
            approbation.RoleApprobateur = etape.NomEtape;

            await _repository.AddApprobationAsync(approbation, cancellationToken);
        }

        _logger.LogInformation("Créé {Count} étapes d'approbation pour le bon {Numero}",
            workflowEtapes.Count, bon.NumeroReference);
    }

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

            // NomEtape contient le RoleCode canonisé
            var requiredRole = GetRequiredRoleFromRoleCode(currentStep.NomEtape);
            var userRoles = _currentUserService.GetUserRoles().ToList();

            if (!UserHasRequiredRole(userRoles, requiredRole))
            {
                return BonSortieResult.Fail($"Vous n'êtes pas autorisé à approuver cette étape. Rôle requis: {requiredRole} (votre étape: {currentStep.NomEtape})");
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
                ? GetPendingStatusForRoleCode(nextStep.NomEtape)
                : "Approved";

            bon.StatutActuel = nextStatut;
            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.Id, ActionBonSortie.Approbation, oldStatut, nextStatut,
                request.Commentaire ?? "Approbation", cancellationToken);

            // BSM-030: Après approbation finale
            if (nextStatut == "Approved")
            {
                // STOCK: Décrémenter les quantités disponibles dans les matériels source
                _logger.LogInformation("Vérification des matériels pour décrémentation stock - BSM {Numero}", bon.NumeroReference);

                if (bon.Materiels.Any(m => m.MaterielEntreeId.HasValue))
                {
                    try
                    {
                        var materielsASortir = bon.Materiels
                            .Where(m => m.MaterielEntreeId.HasValue)
                            .Select(m => new MaterielStockDecrement
                            {
                                MaterielEntreeId = m.MaterielEntreeId!.Value,
                                QuantiteASortir = m.Quantite
                            })
                            .ToList();

                        var stockResult = await _bonEntreeLockService.DecrementStockAsync(materielsASortir, cancellationToken);
                        if (!stockResult.Success)
                        {
                            _logger.LogWarning("Échec de la mise à jour du stock pour BSM {Numero}: {Message}",
                                bon.NumeroReference, stockResult.ErrorMessage);
                        }
                        else
                        {
                            _logger.LogInformation("Stock décrémenté pour {Count} matériels du BSM {Numero}",
                                bon.Materiels.Count(m => m.MaterielEntreeId.HasValue), bon.NumeroReference);
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

            var requiredRole = GetRequiredRoleFromRoleCode(currentStep.NomEtape);
            var userRoles = _currentUserService.GetUserRoles().ToList();

            if (!UserHasRequiredRole(userRoles, requiredRole))
            {
                return BonSortieResult.Fail($"Vous n'êtes pas autorisé à rejeter cette étape. Rôle requis: {requiredRole} (votre étape: {currentStep.NomEtape})");
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

    private static IEnumerable<WorkflowEtapeConfig> BuildDefaultWorkflowEtapes(Domain.Entities.BonSortie bon)
    {
        var typeMateriel = bon switch
        {
            BonSortieExterne ext => ext.TypeMateriel,
            BonSortieInterne inter => inter.TypeMateriel,
            _ => TypeMateriel.Autre
        };

        var etapes = new List<WorkflowEtapeConfig>();
        var ordre = 1;

        if (typeMateriel == TypeMateriel.Informatique)
        {
            etapes.Add(new WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "IT", NomEtape = "IT", EstActif = true });
        }

        if (typeMateriel is TypeMateriel.Residu or TypeMateriel.Radioprotection or TypeMateriel.Modification)
        {
            etapes.Add(new WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "Environnement", NomEtape = "Environnement", EstActif = true });
        }

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
