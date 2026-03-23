using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonEntree.Entities;
using KCCMaterialFlow.Module.BonEntree.Repositories;
using KCCMaterialFlow.Module.Shared.Services;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Module.BonEntree.Services;

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
    private readonly ILogger<BonEntreeService> _logger;

    private const int DefaultValidityDays = 7;

    public BonEntreeService(
        IBonEntreeRepository repository,
        ICurrentUserService currentUserService,
        IQRCodeService qrCodeService,
        IBarriereService barriereService,
        INotificationRejetService notificationRejetService,
        ILogger<BonEntreeService> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _qrCodeService = qrCodeService;
        _barriereService = barriereService;
        _notificationRejetService = notificationRejetService;
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

        var bonEntree = new Entities.BonEntree
        {
            // Champs Bon (classe mère)
            StatutActuel = "Draft",
            Provenance = request.Provenance,
            Destination = request.Destination,
            Description = request.Description,
            DateExpiration = request.DateExpiration != default 
                ? request.DateExpiration 
                : DateTime.Now.AddDays(DefaultValidityDays),
            
            // Champs BonEntree
            NomDemandeur = _currentUserService.GetUserDisplayName(),
            ContratId = request.ContratId,
            NumeroContrat = request.NumeroContrat,
            NomCompagnie = request.NomCompagnie,
            EmailContractant = request.EmailContractant,
            SiteManager = request.SiteManager,
            HostDepartment = request.HostDepartment,
            ReasonOnSite = request.ReasonOnSite,
            NomEscorteur = request.NomEscorteur,
            FonctionEscorteur = request.FonctionEscorteur
        };

        // Ajouter les matériels (3 champs essentiels + stock)
        foreach (var mat in request.Materiels)
        {
            bonEntree.Materiels.Add(new Materiel
            {
                CodeProduitSerial = mat.CodeProduitSerial,
                Designation = mat.Designation,
                Quantite = mat.Quantite,
                QuantiteDisponible = mat.Quantite // Stock initial = quantité entrée
            });
        }

        bonEntree.Quantite = (int)bonEntree.Materiels.Sum(m => m.Quantite);

        var created = await _repository.CreateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(created.IdBon, ActionBonEntree.Creation, null, "Draft",
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
        bonEntree.ReasonOnSite = request.ReasonOnSite;
        bonEntree.NomEscorteur = request.NomEscorteur;
        bonEntree.FonctionEscorteur = request.FonctionEscorteur;
        bonEntree.Provenance = request.Provenance;
        bonEntree.Destination = request.Destination;
        bonEntree.DateExpiration = request.DateExpiration;
        bonEntree.Description = request.Description;

        // Mise à jour des matériels (uniquement en brouillon)
        if (request.Materiels != null && request.Materiels.Any())
        {
            // Supprimer les matériels qui ne sont plus dans la liste
            var requestMaterielIds = request.Materiels.Where(m => m.IdMateriel > 0).Select(m => m.IdMateriel).ToList();
            var materielsToRemove = bonEntree.Materiels.Where(m => !requestMaterielIds.Contains(m.IdMateriel)).ToList();
            foreach (var mat in materielsToRemove)
            {
                bonEntree.Materiels.Remove(mat);
            }

            // Mettre à jour ou ajouter les matériels
            foreach (var matRequest in request.Materiels)
            {
                if (matRequest.IdMateriel > 0)
                {
                    // Mise à jour d'un matériel existant
                    var existingMat = bonEntree.Materiels.FirstOrDefault(m => m.IdMateriel == matRequest.IdMateriel);
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
                    // Nouveau matériel
                    bonEntree.Materiels.Add(new Entities.Materiel
                    {
                        BonId = bonEntree.IdBon,
                        CodeProduitSerial = matRequest.CodeProduitSerial,
                        Designation = matRequest.Designation,
                        Quantite = matRequest.Quantite,
                        QuantiteDisponible = matRequest.Quantite
                    });
                }
            }
        }
        else
        {
            // Si aucun matériel fourni, vider la liste
            bonEntree.Materiels.Clear();
        }

        var updated = await _repository.UpdateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(updated.IdBon, ActionBonEntree.Modification, "Draft", "Draft",
            $"Bon modifié", null, cancellationToken);

        _logger.LogInformation("Bon d'entrée {Numero} modifié par {User}", updated.NumeroReference, CurrentLogin);

        return BonEntreeResult.Ok(updated, "Bon d'entrée mis à jour avec succès");
    }

    public async Task<Entities.BonEntree?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, 
            includeMateriels: true, 
            includeApprobations: true,
            cancellationToken: cancellationToken);
    }

    public async Task<Entities.BonEntree?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default)
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

    public async Task<IReadOnlyList<Entities.BonEntree>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
    {
        var userRoles = _currentUserService.GetUserRoles();
        _logger.LogDebug("GetPendingApprovalsAsync appelé pour {Login} avec rôles: {Roles}", 
            CurrentLogin, string.Join(", ", userRoles));
        return await _repository.GetPendingApprovalsAsync(userRoles, cancellationToken);
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
                    IdBon = bon.IdBon,
                    NumeroReference = bon.NumeroReference,
                    RaisonRetour = dernierRetour.Comment ?? dernierRetour.ActionDescription ?? "",
                    DateRetour = dernierRetour.ActionDate,
                    AuteurRetour = dernierRetour.ActionByNom ?? dernierRetour.ActionBy
                });
            }
        }
        
        return returnedBons.OrderByDescending(r => r.DateRetour).ToList();
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

        await CreateApprovalStepsAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(bonEntree.IdBon, ActionBonEntree.Soumission, oldStatut, "PendingSup",
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
        var currentStep = approbations.FirstOrDefault(a => a.Decision == "En attente");

        if (currentStep == null)
        {
            return BonEntreeResult.Fail("Aucune approbation en attente pour ce bon");
        }

        // Vérifier que le rôle de l'utilisateur correspond à l'étape d'approbation
        var userRoles = _currentUserService.GetUserRoles().ToList();
        var requiredRoleForStep = GetRequiredRoleForStep(currentStep.NomEtape);
        
        if (!userRoles.Contains(requiredRoleForStep))
        {
            return BonEntreeResult.Fail($"Vous n'êtes pas autorisé à approuver cette étape. Rôle requis: {requiredRoleForStep}");
        }

        // Enregistrer les infos de l'approbateur
        var oldStatutApprove = bonEntree.StatutActuel;
        currentStep.Decision = "Approuvé";
        currentStep.DateAction = DateTime.Now;
        currentStep.ReservesEventuelles = request.ReservesEventuelles;
        currentStep.NomApprobateur = _currentUserService.GetUserDisplayName();
        currentStep.RoleApprobateur = requiredRoleForStep;

        await _repository.UpsertApprobationAsync(currentStep, cancellationToken);

        var nextStep = approbations
            .Where(a => a.OrdreEtape > currentStep.OrdreEtape && a.Decision == "En attente")
            .OrderBy(a => a.OrdreEtape)
            .FirstOrDefault();

        if (nextStep != null)
        {
            bonEntree.StatutActuel = GetStatutForEtape(nextStep.OrdreEtape);
        }
        else
        {
            // Approbation finale - Génération automatique du QR Code
            bonEntree.StatutActuel = "Approved";
            
            try
            {
                var qrResult = _qrCodeService.GenerateQRCode(bonEntree.IdBon, "BEM", bonEntree.NumeroReference);
                bonEntree.QRCodeBase64 = qrResult.QRCodeBase64;
                bonEntree.QRCodeHash = qrResult.HashedCode;
                bonEntree.DateGenerationQR = DateTime.Now;
                bonEntree.QRCodeData = $"{{\"IdBon\":{bonEntree.IdBon},\"Type\":\"BEM\",\"Reference\":\"{bonEntree.NumeroReference}\"}}";
                
                _logger.LogInformation("QR Code généré automatiquement pour le bon approuvé {Numero}", bonEntree.NumeroReference);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Échec de la génération du QR Code pour le bon {Numero}", bonEntree.NumeroReference);
                // Ne pas bloquer l'approbation en cas d'erreur de génération QR
            }
        }

        await _repository.UpdateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(bonEntree.IdBon, ActionBonEntree.Approbation, oldStatutApprove, bonEntree.StatutActuel,
            $"Validé par {_currentUserService.GetUserDisplayName()} ({requiredRoleForStep})",
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

        // Vérifier que le rôle de l'utilisateur correspond à l'étape d'approbation
        var userRoles = _currentUserService.GetUserRoles().ToList();
        var requiredRoleForStep = GetRequiredRoleForStep(currentStep.NomEtape);
        
        if (!userRoles.Contains(requiredRoleForStep))
        {
            return BonEntreeResult.Fail($"Vous n'êtes pas autorisé à rejeter cette étape. Rôle requis: {requiredRoleForStep}");
        }

        currentStep.Decision = "Rejeté";
        currentStep.DateAction = DateTime.Now;
        currentStep.ReservesEventuelles = request.ReservesEventuelles;
        currentStep.NomApprobateur = _currentUserService.GetUserDisplayName();
        currentStep.RoleApprobateur = requiredRoleForStep;

        await _repository.UpsertApprobationAsync(currentStep, cancellationToken);

        var oldStatutReject = bonEntree.StatutActuel;
        bonEntree.StatutActuel = "Rejected";
        await _repository.UpdateAsync(bonEntree, cancellationToken);

        await AddHistoryAsync(bonEntree.IdBon, ActionBonEntree.Rejet, oldStatutReject, "Rejected",
            $"Refusé par {_currentUserService.GetUserDisplayName()} ({requiredRoleForStep})",
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

        await AddHistoryAsync(bonEntree.IdBon, ActionBonEntree.RetourModification, oldStatutReturn, "Draft",
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

        var qrResult = _qrCodeService.GenerateQRCode(bonEntree.IdBon, "BEM", bonEntree.NumeroReference);

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
            itineraires.Add(new ItinerairePrevu
            {
                BonId = id,
                OrdrePassage = ordre++,
                BarriereId = barriere.IdBarriere
            });
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
        var history = new BonEntreeHistory
        {
            BonId = bonId,
            Action = action,
            ActionDescription = description,
            ActionBy = _currentUserService.GetUserLogin(),
            ActionByNom = _currentUserService.GetUserDisplayName(),
            ActionDate = DateTime.Now,
            Comment = comment,
            StatutAvant = statutAvant,
            StatutApres = statutApres
        };

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

    private async Task CreateApprovalStepsAsync(Entities.BonEntree bon, CancellationToken cancellationToken)
    {
        // Chaîne d'approbation standard BEM: Superviseur → GM → OPJ → Identification
        var steps = new List<Approbation>
        {
            new() { BonId = bon.IdBon, OrdreEtape = 1, NomEtape = "Superviseur", Decision = "En attente" },
            new() { BonId = bon.IdBon, OrdreEtape = 2, NomEtape = "General Manager", Decision = "En attente" },
            new() { BonId = bon.IdBon, OrdreEtape = 3, NomEtape = "OPJ", Decision = "En attente" },
            new() { BonId = bon.IdBon, OrdreEtape = 4, NomEtape = "Identification", Decision = "En attente" }
        };

        foreach (var step in steps)
        {
            await _repository.UpsertApprobationAsync(step, cancellationToken);
        }
    }

    /// <summary>
    /// Retourne le rôle requis pour une étape d'approbation donnée
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

    #endregion
}
