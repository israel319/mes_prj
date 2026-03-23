using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.Shared.Services;
using TypeMateriel = KCCMaterialFlow.Domain.Enums.TypeMateriel;
using KCCMaterialFlow.Module.BonSortie.Entities;
using KCCMaterialFlow.Module.BonSortie.Repositories;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Module.BonSortie.Services;

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
                // Circulaire: validité max 6 mois, pas de lien BonEntree requis
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

            var bonSortie = new BonSortieExterne
            {
                NumeroReference = numeroReference,
                DateCreation = DateTime.Now,
                DateExpiration = request.DateExpiration,
                StatutActuel = "Draft",
                CreatedByLogin = _currentUserService.GetUserLogin(),
                Provenance = request.Provenance,
                Destination = request.Destination,
                Description = request.Description,
                NomDemandeur = request.NomDemandeur,
                FonctionDemandeur = request.FonctionDemandeur,
                DepartementDemandeur = request.DepartementDemandeur,
                MotifSortie = request.MotifSortie,
                RaisonSortieCode = resolvedRaisonCode,
                EstDefinitif = true,
                BonEntreeAssocieId = request.BonEntreeAssocieId,
                TypeMateriel = typeMateriel,
                NomDestinataire = request.NomDestinataire,
                AdresseDestination = request.AdresseDestination,
                NumeroVehicule = request.NumeroVehicule,
                NomChauffeur = request.NomChauffeur,
                TelephoneChauffeur = request.TelephoneChauffeur
            };

            // Ajouter les matériels
            foreach (var mat in request.Materiels)
            {
                bonSortie.Materiels.Add(new MaterielSortie
                {
                    CodeProduitSerial = mat.CodeProduitSerial,
                    Designation = mat.Designation,
                    Quantite = mat.Quantite,
                    MaterielEntreeId = mat.MaterielEntreeId,
                    BonEntreeId = mat.BonEntreeId,
                    BonEntreeNumero = mat.BonEntreeNumero,
                    QuantiteInitialeBem = mat.QuantiteInitialeBem,
                    QuantiteDisponible = mat.QuantiteDisponible.HasValue 
                        ? mat.QuantiteDisponible - mat.Quantite  // Reliquat après sortie
                        : null,
                    Observations = mat.Observations
                });
            }

            bonSortie.Quantite = bonSortie.Materiels.Count;

            var created = await _repository.AddAsync(bonSortie, cancellationToken);

            // Historique
            await AddHistoryAsync(created.IdBon, "Création", null, "Draft", 
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

            var bonSortie = new BonSortieInterne
            {
                NumeroReference = numeroReference,
                DateCreation = DateTime.Now,
                DateExpiration = request.DateExpiration,
                StatutActuel = "Draft",
                CreatedByLogin = _currentUserService.GetUserLogin(),
                Provenance = request.Provenance,
                Destination = request.Destination,
                Description = request.Description,
                NomDemandeur = request.NomDemandeur,
                FonctionDemandeur = request.FonctionDemandeur,
                DepartementDemandeur = request.DepartementDemandeur,
                MotifSortie = request.MotifSortie,
                RaisonSortieCode = resolvedRaisonCode,
                EstDefinitif = true,
                BonEntreeAssocieId = request.BonEntreeAssocieId,
                TypeMateriel = typeMateriel,
                DepartementOrigine = request.DepartementOrigine,
                FonctionReceveur = request.FonctionReceveur,
                EmailReceveur = request.EmailReceveur,
                LocalisationDestination = request.LocalisationDestination,
                DateTransfertPrevue = request.DateTransfertPrevue
            };

            foreach (var mat in request.Materiels)
            {
                bonSortie.Materiels.Add(new MaterielSortie
                {
                    CodeProduitSerial = mat.CodeProduitSerial,
                    Designation = mat.Designation,
                    Quantite = mat.Quantite,
                    MaterielEntreeId = mat.MaterielEntreeId,
                    BonEntreeId = mat.BonEntreeId,
                    BonEntreeNumero = mat.BonEntreeNumero,
                    QuantiteInitialeBem = mat.QuantiteInitialeBem,
                    QuantiteDisponible = mat.QuantiteDisponible.HasValue 
                        ? mat.QuantiteDisponible - mat.Quantite
                        : null,
                    Observations = mat.Observations
                });
            }

            bonSortie.Quantite = bonSortie.Materiels.Count;

            var created = await _repository.AddAsync(bonSortie, cancellationToken);

            await AddHistoryAsync(created.IdBon, "Création", null, "Draft",
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
            // Récupérer le code de la raison si RaisonId est fourni
            string? raisonCode = null;
            if (request.RaisonId.HasValue)
            {
                raisonCode = await _repository.GetRaisonSortieCodeByIdAsync(request.RaisonId.Value, cancellationToken);
            }
            else
            {
                // Par défaut pour un prêt, utiliser le code PRET
                raisonCode = "PRET";
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

            var pret = new Pret
            {
                NumeroReference = numeroReference,
                DateCreation = DateTime.Now,
                DateExpiration = request.DateExpiration,
                StatutActuel = "Draft",
                CreatedByLogin = _currentUserService.GetUserLogin(),
                Provenance = request.Provenance,
                Destination = request.Destination,
                Description = request.Description,
                NomDemandeur = request.NomDemandeur,
                FonctionDemandeur = request.FonctionDemandeur,
                DepartementDemandeur = request.DepartementDemandeur,
                MotifSortie = request.MotifSortie,
                RaisonSortieCode = resolvedRaisonCode,
                EstDefinitif = false, // Prêt = temporaire
                BonEntreeAssocieId = request.BonEntreeAssocieId,
                TypeMateriel = typeMateriel,
                NomDestinataire = request.NomDestinataire,
                AdresseDestination = request.AdresseDestination,
                NumeroVehicule = request.NumeroVehicule,
                NomChauffeur = request.NomChauffeur,
                TelephoneChauffeur = request.TelephoneChauffeur,
                DateRetourPrevue = request.DateRetourPrevue ?? DateTime.Now.AddMonths(1),
                EstRetourne = false
            };

            foreach (var mat in request.Materiels)
            {
                pret.Materiels.Add(new MaterielSortie
                {
                    CodeProduitSerial = mat.CodeProduitSerial,
                    Designation = mat.Designation,
                    Quantite = mat.Quantite,
                    MaterielEntreeId = mat.MaterielEntreeId,
                    BonEntreeId = mat.BonEntreeId,
                    BonEntreeNumero = mat.BonEntreeNumero,
                    QuantiteInitialeBem = mat.QuantiteInitialeBem,
                    QuantiteDisponible = mat.QuantiteDisponible.HasValue 
                        ? mat.QuantiteDisponible - mat.Quantite
                        : null,
                    Observations = mat.Observations
                });
            }

            pret.Quantite = pret.Materiels.Count;

            var created = await _repository.AddAsync(pret, cancellationToken);

            await AddHistoryAsync(created.IdBon, "Création", null, "Draft",
                $"Création du prêt {numeroReference} - Retour prévu: {request.DateRetourPrevue:dd/MM/yyyy}", cancellationToken);

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

            // Mise à jour des champs communs
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

            // Mise à jour des champs spécifiques BonSortieExterne
            if (bon is BonSortieExterne externe)
            {
                if (!string.IsNullOrEmpty(request.NomDestinataire))
                    externe.NomDestinataire = request.NomDestinataire;

                if (!string.IsNullOrEmpty(request.AdresseDestination))
                    externe.AdresseDestination = request.AdresseDestination;

                if (!string.IsNullOrEmpty(request.NumeroVehicule))
                    externe.NumeroVehicule = request.NumeroVehicule;

                if (!string.IsNullOrEmpty(request.NomChauffeur))
                    externe.NomChauffeur = request.NomChauffeur;

                if (!string.IsNullOrEmpty(request.TelephoneChauffeur))
                    externe.TelephoneChauffeur = request.TelephoneChauffeur;
            }

            // Mise à jour des champs spécifiques Prêt
            if (bon is Pret pret && request.DateRetourPrevue.HasValue)
            {
                pret.DateRetourPrevue = request.DateRetourPrevue.Value;
            }

            // Mise à jour des matériels
            if (request.Materiels != null && request.Materiels.Any())
            {
                // Supprimer explicitement les anciens matériels de la BD
                if (bon.Materiels.Any())
                {
                    await _repository.RemoveMaterielsAsync(bon.IdBon, cancellationToken);
                }
                bon.Materiels.Clear();
                
                // Ajouter les nouveaux matériels
                foreach (var mat in request.Materiels)
                {
                    bon.Materiels.Add(new MaterielSortie
                    {
                        BonSortieId = bon.IdBon,
                        CodeProduitSerial = mat.CodeProduitSerial,
                        Designation = mat.Designation,
                        Quantite = mat.Quantite,
                        MaterielEntreeId = mat.MaterielEntreeId,
                        BonEntreeId = mat.BonEntreeId,
                        BonEntreeNumero = mat.BonEntreeNumero,
                        QuantiteInitialeBem = mat.QuantiteInitialeBem,
                        QuantiteDisponible = mat.QuantiteDisponible.HasValue 
                            ? mat.QuantiteDisponible - mat.Quantite
                            : null,
                        Observations = mat.Observations
                    });
                }
                bon.Quantite = bon.Materiels.Count;
            }

            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.IdBon, "Modification", bon.StatutActuel ?? "Draft", bon.StatutActuel ?? "Draft",
                "Modification du bon de sortie", cancellationToken);

            return BonSortieResult.Ok(bon, "Bon de sortie mis à jour");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du bon {Id}", request.IdBon);
            return BonSortieResult.Fail($"Erreur: {ex.Message}");
        }
    }

    public async Task<Entities.BonSortie?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Entities.BonSortie?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default)
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
            cancellationToken);

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

            await AddHistoryAsync(bon.IdBon, "Annulation", oldStatut, "Cancelled",
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

            // Vérifier que l'utilisateur est le créateur ou admin
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
        var expectedHostDepartment = ResolveExpectedHostDepartment(raisonSortieCode, typeMateriel);
        if (string.IsNullOrWhiteSpace(expectedHostDepartment))
        {
            return null;
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

        if (!hostDepartment.Contains(expectedHostDepartment, StringComparison.OrdinalIgnoreCase))
        {
            return $"Incohérence métier: le motif '{raisonSortieCode ?? typeMateriel.ToString()}' requiert un BEM du département '{expectedHostDepartment}', mais le BEM sélectionné est rattaché à '{hostDepartment}'.";
        }

        return null;
    }

    private static string? ResolveExpectedHostDepartment(string? raisonSortieCode, TypeMateriel typeMateriel)
    {
        var normalizedRaison = raisonSortieCode?.Trim().ToUpperInvariant();
        if (normalizedRaison == "INFO") return "IT";
        if (normalizedRaison is "RESIDU" or "RADIO_PROT" or "MODIF") return "ENV";

        return typeMateriel switch
        {
            TypeMateriel.Informatique => "IT",
            TypeMateriel.Residu or TypeMateriel.Radioprotection or TypeMateriel.Modification => "ENV",
            _ => null
        };
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

    public async Task<IReadOnlyList<Entities.BonSortie>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
    {
        var roles = _currentUserService.GetUserRoles();
        var allPending = new List<Entities.BonSortie>();

        foreach (var role in roles)
        {
            var pending = await _repository.GetPendingApprovalAsync(role, cancellationToken);
            allPending.AddRange(pending);
        }

        return allPending.DistinctBy(b => b.IdBon).ToList();
    }

    public async Task<IReadOnlyList<Entities.ApprobationSortie>> GetApprobationsAsync(int bonId, CancellationToken cancellationToken = default)
    {
        var bon = await _repository.GetByIdAsync(bonId, cancellationToken);
        if (bon == null)
            return Array.Empty<Entities.ApprobationSortie>();

        return bon.Approbations?.OrderBy(a => a.OrdreEtape).ToList() ?? new List<Entities.ApprobationSortie>();
    }

    public async Task<IReadOnlyList<ReturnedBonInfo>> GetMyReturnedBonsAsync(CancellationToken cancellationToken = default)
    {
        // Récupérer tous les bons de l'utilisateur
        var myBonsResult = await GetMyBonsAsync(0, 100, cancellationToken);
        
        var returnedBons = new List<ReturnedBonInfo>();
        
        foreach (var bon in myBonsResult.Items.Where(b => b.StatutActuel == "Draft"))
        {
            // Vérifier s'il y a un historique de retour
            var dernierRetour = bon.Historiques
                .Where(h => h.TypeAction == "Retour")
                .OrderByDescending(h => h.DateAction)
                .FirstOrDefault();
            
            if (dernierRetour != null)
            {
                returnedBons.Add(new ReturnedBonInfo
                {
                    IdBon = bon.IdBon,
                    NumeroReference = bon.NumeroReference,
                    TypeBon = "BSM",
                    RaisonRetour = dernierRetour.Description?.Replace("Retourné pour modification: ", "") ?? "",
                    DateRetour = dernierRetour.DateAction,
                    AuteurRetour = dernierRetour.UtilisateurNom ?? ""
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

            // Vérifier que l'utilisateur actuel est le propriétaire du brouillon
            var currentLogin = _currentUserService.GetUserLogin();
            if (!string.IsNullOrEmpty(bon.CreatedByLogin) && bon.CreatedByLogin != currentLogin)
                return BonSortieResult.Fail("Vous ne pouvez soumettre que vos propres brouillons");

            if (!bon.Materiels.Any())
                return BonSortieResult.Fail("Ajoutez au moins un matériel avant de soumettre");

            // Créer les étapes d'approbation selon le workflow
            await CreateApprovalStepsAsync(bon, cancellationToken);

            if (!bon.Approbations.Any())
                return BonSortieResult.Fail("Aucune étape de workflow n'a été configurée pour ce motif");

            var oldStatut = bon.StatutActuel;
            var firstStep = bon.Approbations.OrderBy(a => a.OrdreEtape).First();
            // RoleCode est maintenant stocké directement dans ApprobationSortie — lecture directe.
            var newStatut = GetPendingStatusForRoleCode(firstStep.RoleCode);
            bon.StatutActuel = newStatut;

            await _repository.UpdateAsync(bon, cancellationToken);

            var etapeDescription = $"Soumis à l'étape: {firstStep.NomEtape}";

            await AddHistoryAsync(bon.IdBon, "Soumission", oldStatut, newStatut,
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
    /// Priorité: configuration par motif (RaisonSortieCode), sinon fallback workflow par défaut.
    /// </summary>
    private async Task CreateApprovalStepsAsync(Entities.BonSortie bon, CancellationToken cancellationToken)
    {
        // Supprimer les anciennes approbations si existantes
        bon.Approbations.Clear();

        var workflowEtapes = await _workflowConfigService.GetResolvedWorkflowEtapesAsync("BSM", bon.RaisonSortieCode, cancellationToken);

        if (workflowEtapes.Count == 0)
        {
            workflowEtapes = BuildDefaultWorkflowEtapes(bon).ToList();
        }

        var ordreEtape = 1;
        foreach (var etape in workflowEtapes.OrderBy(x => x.OrdreEtape))
        {
            // RoleCode est stocké directement depuis le workflow configuré en BD.
            // C'est la source de vérité pour l'autorisation — jamais re-déduit au runtime.
            bon.Approbations.Add(new Entities.ApprobationSortie
            {
                BonSortieId = bon.IdBon,
                OrdreEtape = ordreEtape++,
                // On stocke la forme CANONIQUE pour que les requêtes BD soient directes et portables.
                // Ex: "Département IT" → "IT", "General Manager" → "GM"
                RoleCode   = CanonicalizeRoleCode(etape.RoleCode),
                NomEtape   = etape.NomEtape,
                Decision   = "En attente"
            });
        }
        
        _logger.LogInformation("Créé {Count} étapes d'approbation pour le bon {Numero}",
            bon.Approbations.Count, bon.NumeroReference);
    }

    public async Task<BonSortieResult> ApproveAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
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

            // Vérification du rôle : on lit RoleCode directement depuis l'étape en BD.
            // Jamais dérivé d'un switch ou d'un re-lookup workflow.
            var requiredRole = GetRequiredRoleFromRoleCode(currentStep.RoleCode);
            var userRoles = _currentUserService.GetUserRoles().ToList();

            if (!UserHasRequiredRole(userRoles, requiredRole))
            {
                return BonSortieResult.Fail($"Vous n'êtes pas autorisé à approuver cette étape. Rôle requis: {requiredRole} (votre étape: {currentStep.NomEtape})");
            }

            var oldStatut = bon.StatutActuel;
            var nomEtape = currentStep.NomEtape;

            // Ajouter ou mettre à jour l'approbation
            var approbation = bon.Approbations.FirstOrDefault(a => a.IdApprobation == currentStep.IdApprobation);
            if (approbation == null)
            {
                // Créer une nouvelle approbation
                var ordreEtape = bon.Approbations.Count + 1;
                approbation = new Entities.ApprobationSortie
                {
                    BonSortieId = bon.IdBon,
                    OrdreEtape = ordreEtape,
                    NomEtape = nomEtape
                };
                bon.Approbations.Add(approbation);
            }
            
            // Mettre à jour avec les informations de l'approbateur
            approbation.Decision = "Approuvé";
            approbation.DateAction = DateTime.Now;
            approbation.ApprobateurLogin = _currentUserService.GetUserLogin();
            approbation.ApprobateurNom = _currentUserService.GetUserDisplayName();
            approbation.Commentaire = request.Commentaire;

            var nextStep = bon.Approbations
                .Where(a => a.Decision == "En attente")
                .OrderBy(a => a.OrdreEtape)
                .FirstOrDefault();

            // Prochaine étape : son RoleCode est directement disponible depuis la BD.
            var nextStatut = nextStep != null
                ? GetPendingStatusForRoleCode(nextStep.RoleCode)
                : "Approved";

            bon.StatutActuel = nextStatut;
            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.IdBon, "Approbation", oldStatut, nextStatut,
                request.Commentaire ?? "Approbation", cancellationToken);

            // BSM-030: Génération automatique du QR Code après approbation finale (Identification)
            if (nextStatut == "Approved")
            {
                // STOCK: Décrémenter les quantités disponibles dans les matériels source
                _logger.LogInformation("Vérification des matériels pour décrémentation stock - BSM {Numero}", bon.NumeroReference);
                _logger.LogInformation("Nombre total de matériels: {Count}", bon.Materiels.Count);
                
                foreach (var mat in bon.Materiels)
                {
                    _logger.LogInformation("Matériel: {Designation}, MaterielEntreeId={EntreeId}, Quantite={Qte}", 
                        mat.Designation, mat.MaterielEntreeId?.ToString() ?? "NULL", mat.Quantite);
                }

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
                        
                        _logger.LogInformation("Appel de DecrementStockAsync avec {Count} matériels", materielsASortir.Count);

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

                // BSM-031: Verrouiller le BonEntree associé (si applicable)
                if (bon is BonSortieExterne externe && externe.BonEntreeAssocieId.HasValue)
                {
                    try
                    {
                        await _bonEntreeLockService.LockAsync(
                            externe.BonEntreeAssocieId.Value,
                            bon.IdBon,
                            bon.NumeroReference,
                            cancellationToken);

                        _logger.LogInformation("BSM-031: BEM {BemId} verrouillé pour BSM {BsmNumero}",
                            externe.BonEntreeAssocieId, bon.NumeroReference);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Échec du verrouillage du BEM {BemId} pour BSM {BsmNumero}",
                            externe.BonEntreeAssocieId, bon.NumeroReference);
                        // Ne pas bloquer l'approbation en cas d'erreur de verrouillage
                    }
                }

                // BSM-030: Génération du QR Code
                var qrResult = await GenerateQRCodeAsync(bon.IdBon, cancellationToken);
                if (!qrResult.Success)
                {
                    _logger.LogWarning("Échec de la génération du QR Code pour le bon {Numero}: {Message}",
                        bon.NumeroReference, qrResult.Message);
                }
                else
                {
                    // Recharger le bon avec les données QR Code mises à jour
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

            // Vérification du rôle pour le rejet : lecture directe de RoleCode depuis la BD.
            var requiredRole = GetRequiredRoleFromRoleCode(currentStep.RoleCode);
            var userRoles = _currentUserService.GetUserRoles().ToList();

            if (!UserHasRequiredRole(userRoles, requiredRole))
            {
                return BonSortieResult.Fail($"Vous n'êtes pas autorisé à rejeter cette étape. Rôle requis: {requiredRole} (votre étape: {currentStep.NomEtape})");
            }

            var oldStatut = bon.StatutActuel;
            var nomEtape = currentStep.NomEtape;

            // Ajouter ou mettre à jour l'approbation avec le rejet
            var approbation = bon.Approbations.FirstOrDefault(a => a.IdApprobation == currentStep.IdApprobation);
            if (approbation == null)
            {
                var ordreEtape = bon.Approbations.Count + 1;
                approbation = new Entities.ApprobationSortie
                {
                    BonSortieId = bon.IdBon,
                    OrdreEtape = ordreEtape,
                    NomEtape = nomEtape
                };
                bon.Approbations.Add(approbation);
            }
            
            // Mettre à jour avec les informations du rejet
            approbation.Decision = "Rejeté";
            approbation.DateAction = DateTime.Now;
            approbation.ApprobateurLogin = _currentUserService.GetUserLogin();
            approbation.ApprobateurNom = _currentUserService.GetUserDisplayName();
            approbation.Commentaire = request.Motif;

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
            
            // Enregistrer la notification de rejet en base de données
            try
            {
                // Extraire le type de bon depuis NumeroReference (BSE, BSI, PRT...)
                var bonTypePrefix = bon.NumeroReference.Split('-').FirstOrDefault() ?? "BSM";
                await _notificationRejetService.EnregistrerRejetAsync(
                    bonType: bonTypePrefix,
                    numeroReference: bon.NumeroReference,
                    etapeRejet: nomEtape,
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

            await AddHistoryAsync(bon.IdBon, "Rejet", oldStatut, "Rejected",
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

    public async Task<BonSortieResult> ReturnForModificationAsync(ApprovalRequest request, CancellationToken cancellationToken = default)
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

            var nomEtape = currentStep.NomEtape;

            // Ajouter ou mettre à jour l'approbation avec le retour
            var approbation = bon.Approbations.FirstOrDefault(a => a.IdApprobation == currentStep.IdApprobation);
            if (approbation == null)
            {
                var ordreEtape = bon.Approbations.Count + 1;
                approbation = new Entities.ApprobationSortie
                {
                    BonSortieId = bon.IdBon,
                    OrdreEtape = ordreEtape,
                    NomEtape = nomEtape
                };
                bon.Approbations.Add(approbation);
            }
            
            // Mettre à jour avec les informations du retour
            approbation.Decision = "Retourné";
            approbation.DateAction = DateTime.Now;
            approbation.ApprobateurLogin = _currentUserService.GetUserLogin();
            approbation.ApprobateurNom = _currentUserService.GetUserDisplayName();
            approbation.Commentaire = request.Commentaire;

            // Remettre le bon en brouillon pour modification par le demandeur
            bon.StatutActuel = "Draft";

            // Effacer les approbations pour permettre un nouveau cycle
            bon.Approbations.Clear();

            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.IdBon, "Retour", oldStatut, "Draft",
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

    private string GetPendingStatusForRoleCode(string roleCode)
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

    /// <summary>
    /// Retourne le rôle utilisateur requis pour approuver/rejeter une étape selon le statut du bon
    /// </summary>
    private string GetRequiredRoleFromRoleCode(string roleCode)
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

    private async Task<string> ResolveRoleCodeForStepAsync(Entities.BonSortie bon, Entities.ApprobationSortie step, CancellationToken cancellationToken)
    {
        var workflowEtapes = await _workflowConfigService.GetResolvedWorkflowEtapesAsync("BSM", bon.RaisonSortieCode, cancellationToken);
        var workflowStep = workflowEtapes
            .OrderBy(x => x.OrdreEtape)
            .ElementAtOrDefault(Math.Max(0, step.OrdreEtape - 1));

        if (!string.IsNullOrWhiteSpace(workflowStep?.RoleCode))
        {
            return workflowStep.RoleCode;
        }

        return step.NomEtape;
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

    private static IEnumerable<Module.Shared.Entities.WorkflowEtapeConfig> BuildDefaultWorkflowEtapes(Entities.BonSortie bon)
    {
        var typeMateriel = bon is BonSortieExterne ext ? ext.TypeMateriel :
                           bon is BonSortieInterne inter ? inter.TypeMateriel :
                           bon is Pret pret ? pret.TypeMateriel : TypeMateriel.Autre;

        var etapes = new List<Module.Shared.Entities.WorkflowEtapeConfig>();
        var ordre = 1;

        if (typeMateriel == TypeMateriel.Informatique)
        {
            etapes.Add(new Module.Shared.Entities.WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "IT", NomEtape = "IT", EstActif = true });
        }

        if (typeMateriel == TypeMateriel.Residu || typeMateriel == TypeMateriel.Radioprotection || typeMateriel == TypeMateriel.Modification)
        {
            etapes.Add(new Module.Shared.Entities.WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "Environnement", NomEtape = "Environnement", EstActif = true });
        }

        etapes.Add(new Module.Shared.Entities.WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "Superviseur", NomEtape = "Superviseur", EstActif = true });
        etapes.Add(new Module.Shared.Entities.WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "GM", NomEtape = "General Manager", EstActif = true });
        etapes.Add(new Module.Shared.Entities.WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "OPJ", NomEtape = "OPJ", EstActif = true });
        etapes.Add(new Module.Shared.Entities.WorkflowEtapeConfig { OrdreEtape = ordre++, RoleCode = "Identification", NomEtape = "Identification", EstActif = true });

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

            pret.EstRetourne = true;
            pret.DateRetourEffective = DateTime.Now;
            pret.EtatRetour = request.EtatRetour;
            pret.ReceptionnePar = request.ReceptionnePar ?? _currentUserService.GetUserDisplayName();
            pret.StatutActuel = "Completed";

            await _repository.UpdateAsync(pret, cancellationToken);

            var retardInfo = pret.JoursRetard > 0 ? $" ({pret.JoursRetard} jours de retard)" : "";
            await AddHistoryAsync(pret.IdBon, "Retour", "Approved", "Completed",
                $"Matériel retourné{retardInfo}. État: {request.EtatRetour ?? "Non spécifié"}", cancellationToken);

            // INT-006: Archiver le BEM associé si présent
            if (pret.BonEntreeAssocieId.HasValue)
            {
                try
                {
                    await _bonEntreeLockService.ArchiveAfterSortieAsync(
                        pret.BonEntreeAssocieId.Value, 
                        pret.NumeroReference, 
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "INT-006: Échec de l'archivage du BEM {BemId} après retour du prêt {BsmNumero}",
                        pret.BonEntreeAssocieId, pret.NumeroReference);
                    // Ne pas faire échouer le retour si l'archivage échoue
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

    /// <summary>
    /// BSM-027: Récupère les prêts expirant dans N jours (pour notification J-7)
    /// </summary>
    public async Task<IReadOnlyList<Pret>> GetLoansExpiringInDaysAsync(int days, CancellationToken cancellationToken = default)
    {
        return await _repository.GetLoansExpiringInDaysAsync(days, cancellationToken);
    }

    /// <summary>
    /// BSM-029: Extension de la date de retour d'un prêt (uniquement par Identification)
    /// </summary>
    public async Task<BonSortieResult> ExtendLoanAsync(ExtendLoanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Vérifier que l'utilisateur a le rôle Identification
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
            pret.DateRetourPrevue = request.NouvelleDateRetour;
            pret.DateExpiration = request.NouvelleDateRetour;

            await _repository.UpdateAsync(pret, cancellationToken);

            await AddHistoryAsync(pret.IdBon, "Extension", "Approved", "Approved",
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

    /// <summary>
    /// BSM-028: Envoie manuellement les alertes pour les prêts expirant
    /// </summary>
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
                        pret.IdBon,
                        pret.DateRetourPrevue,
                        joursRestants,
                        cancellationToken);
                    alertsSent++;

                    _logger.LogInformation("Alerte envoyée pour prêt {Numero} - J{Days}",
                        pret.NumeroReference, joursRestants >= 0 ? $"-{joursRestants}" : $"+{-joursRestants}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur envoi alerte pour prêt {Id}", pret.IdBon);
                }
            }

            // Envoyer aussi les alertes pour les prêts en retard
            var overdueLoans = await _repository.GetOverdueLoansAsync(cancellationToken);
            foreach (var pret in overdueLoans)
            {
                try
                {
                    var joursRetard = (DateTime.Today - pret.DateRetourPrevue.Date).Days;
                    await _emailService.SendPretExpirationAlertAsync(
                        pret.IdBon,
                        pret.DateRetourPrevue,
                        -joursRetard, // Négatif pour indiquer le retard
                        cancellationToken);
                    alertsSent++;

                    _logger.LogWarning("Alerte RETARD envoyée pour prêt {Numero} - {Days} jours de retard",
                        pret.NumeroReference, joursRetard);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur envoi alerte retard pour prêt {Id}", pret.IdBon);
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

    // Note: GetOverdueLoansAsync est déjà implémenté dans la région Prêts

    #endregion

    #region QR Code (BSM-030)

    /// <summary>
    /// BSM-030: Génère le QR Code pour un bon de sortie approuvé
    /// Le QR Code est généré automatiquement après l'approbation finale (Identification)
    /// </summary>
    public async Task<BonSortieResult> GenerateQRCodeAsync(int bonId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bon = await _repository.GetByIdAsync(bonId, cancellationToken);
            if (bon == null)
                return BonSortieResult.Fail("Bon de sortie non trouvé");

            // Vérifier que le bon est approuvé
            if (bon.StatutActuel != "Approved")
                return BonSortieResult.Fail("Le QR Code ne peut être généré que pour les bons approuvés");

            // Vérifier si le QR Code n'existe pas déjà
            if (!string.IsNullOrEmpty(bon.QRCodeHash))
            {
                _logger.LogWarning("QR Code déjà généré pour le bon {Numero}", bon.NumeroReference);
                return BonSortieResult.Ok(bon, "Le QR Code existe déjà");
            }

            // Déterminer le type de bon pour le QR Code
            // Note: Pret hérite de BonSortieExterne, donc on le vérifie en premier
            var bonType = bon switch
            {
                Pret => "PRT",
                BonSortieExterne => "BSE",
                BonSortieInterne => "BSI",
                _ => "BSM"
            };

            // Générer le QR Code via le service
            var qrResult = _qrCodeService.GenerateQRCode(bon.IdBon, bonType, bon.NumeroReference);

            // Mettre à jour le bon avec les données QR
            bon.QRCodeBase64 = qrResult.QRCodeBase64;
            bon.QRCodeHash = qrResult.HashedCode;
            bon.QRCodeData = $"{{\"Id\":{bon.IdBon},\"Type\":\"{bonType}\",\"Ref\":\"{bon.NumeroReference}\",\"Hash\":\"{qrResult.HashedCode}\"}}";
            bon.DateGenerationQR = qrResult.GeneratedAt;

            await _repository.UpdateAsync(bon, cancellationToken);

            await AddHistoryAsync(bon.IdBon, "QRCodeGenerated", bon.StatutActuel, bon.StatutActuel,
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

    /// <summary>
    /// BSM-030: Valide un QR Code scanné aux barrières
    /// </summary>
    public async Task<QRCodeValidationResult> ValidateQRCodeAsync(string scannedCode, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(scannedCode))
                return QRCodeValidationResult.Invalid("Code scanné vide");

            // Valider via le service QR Code
            var validationResult = await _qrCodeService.ValidateQRCodeAsync(scannedCode);
            if (!validationResult.IsValid)
                return QRCodeValidationResult.Invalid(validationResult.ErrorMessage ?? "QR Code invalide");

            // Récupérer le bon associé par le hash
            var bon = await _repository.GetByQRCodeHashAsync(scannedCode, cancellationToken);
            if (bon == null)
            {
                // Essayer de décoder le hash pour extraire l'ID
                var decodedInfo = _qrCodeService.DecodeHash(scannedCode);
                if (decodedInfo != null && decodedInfo.BonId > 0)
                {
                    bon = await _repository.GetByIdAsync(decodedInfo.BonId, cancellationToken);
                }
            }

            if (bon == null)
                return QRCodeValidationResult.Invalid("Aucun bon de sortie associé à ce QR Code");

            // Vérifier le statut du bon
            if (bon.StatutActuel != "Approved")
                return QRCodeValidationResult.Invalid($"Le bon n'est pas approuvé (statut: {bon.StatutActuel})");

            // Vérifier l'expiration
            var result = QRCodeValidationResult.Valid(bon);
            if (result.IsExpired)
            {
                result.Message = $"⚠️ QR Code valide mais le bon a expiré le {bon.DateExpiration:dd/MM/yyyy}";
            }
            else
            {
                result.Message = $"✓ Bon valide jusqu'au {bon.DateExpiration:dd/MM/yyyy}";
            }

            _logger.LogInformation("QR Code validé pour le bon {Numero} (expiré: {Expired})",
                bon.NumeroReference, result.IsExpired);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du QR Code");
            return QRCodeValidationResult.Invalid($"Erreur de validation: {ex.Message}");
        }
    }

    #endregion

    #region Liaison Entrée-Sortie (BSM-031)

    /// <summary>
    /// BSM-031: Récupère les informations du bon d'entrée associé
    /// </summary>
    public async Task<BonEntreeBasicInfo?> GetBonEntreeAssocieAsync(int bonSortieId, CancellationToken cancellationToken = default)
    {
        var bon = await _repository.GetByIdAsync(bonSortieId, cancellationToken);

        if (bon is not BonSortieExterne externe || !externe.BonEntreeAssocieId.HasValue)
            return null;

        return await _bonEntreeLockService.GetBasicInfoAsync(externe.BonEntreeAssocieId.Value, cancellationToken);
    }

    /// <summary>
    /// BSM-031: Vérifie la disponibilité d'un bon d'entrée
    /// </summary>
    public async Task<BonEntreeAvailabilityResult> CheckBonEntreeAvailabilityAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        return await _bonEntreeLockService.CheckAvailabilityAsync(bonEntreeId, cancellationToken);
    }

    /// <summary>
    /// Récupère les détails complets d'un BonEntree pour créer un BonSortie
    /// </summary>
    public async Task<DTOs.BonEntreeDetailsPourSortieDto?> GetBonEntreeDetailsForSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        var details = await _bonEntreeLockService.GetDetailsForSortieAsync(bonEntreeId, cancellationToken);
        if (details == null)
            return null;

        return new DTOs.BonEntreeDetailsPourSortieDto
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
            Materiels = details.Materiels.Select(m => new DTOs.MaterielPourSortieDto
            {
                IdMateriel = m.IdMateriel,
                CodeProduitSerial = m.CodeProduitSerial,
                Designation = m.Designation,
                QuantiteInitiale = m.QuantiteInitiale,
                QuantiteDejaSortie = m.QuantiteDejaSortie
            }).ToList()
        };
    }

    /// <summary>
    /// Recherche un BonEntree par son numéro de référence
    /// </summary>
    public async Task<DTOs.BonEntreeDetailsPourSortieDto?> SearchBonEntreeByReferenceAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        var details = await _bonEntreeLockService.SearchByNumeroReferenceAsync(numeroReference, cancellationToken);
        if (details == null)
            return null;

        return new DTOs.BonEntreeDetailsPourSortieDto
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
            Materiels = details.Materiels.Select(m => new DTOs.MaterielPourSortieDto
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

        // Statuts en attente selon la chaîne d'approbation
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

    /// <summary>
    /// INT-005: Récupère les bons de sortie liés à un bon d'entrée
    /// </summary>
    public async Task<IReadOnlyList<BonSortieLieInfo>> GetBonsSortieByBonEntreeAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        var bons = await _repository.GetByBonEntreeAsync(bonEntreeId, cancellationToken);

        return bons.Select(b => new BonSortieLieInfo
        {
            IdBonSortie = b.IdBon,
            NumeroReference = b.NumeroReference,
            TypeBon = b is Pret ? "Prêt" : "Externe",
            StatutActuel = b.StatutActuel,
            DateCreation = b.DateCreation,
            Destination = b.Destination,
            NomDemandeur = b.NomDemandeur,
            NombreMateriels = b.Materiels?.Count ?? 0
        }).ToList();
    }

    #endregion

    #region Private Methods

    private async Task AddHistoryAsync(int bonSortieId, string typeAction, string? statutAvant, string? statutApres, string description, CancellationToken cancellationToken)
    {
        var history = new BonSortieHistory
        {
            BonSortieId = bonSortieId,
            TypeAction = typeAction,
            StatutAvant = statutAvant,
            StatutApres = statutApres,
            Description = description,
            UtilisateurLogin = _currentUserService.GetUserLogin(),
            UtilisateurNom = _currentUserService.GetUserDisplayName(),
            DateAction = DateTime.Now
        };

        await _repository.AddHistoryAsync(history, cancellationToken);
    }

    #endregion
}
