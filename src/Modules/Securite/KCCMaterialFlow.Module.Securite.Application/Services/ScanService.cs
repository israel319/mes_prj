using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.Securite.Entities;
using KCCMaterialFlow.Module.Securite.Repositories;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Module.Securite.Services;

/// <summary>
/// SEC-012: Service métier pour les scans de sécurité.
/// Implémente la validation QR, vérification d'itinéraire, unicité et détection d'anomalies.
/// Utilise IDbContextFactory pour éviter les problèmes de concurrence dans Blazor Server.
/// </summary>
public class ScanService : IScanService
{
    private readonly IScanRepository _scanRepository;
    private readonly IAnomalieRepository _anomalieRepository;
    private readonly IQRCodeService _qrCodeService;
    private readonly ILogger<ScanService> _logger;

    public ScanService(
        IScanRepository scanRepository,
        IAnomalieRepository anomalieRepository,
        IQRCodeService qrCodeService,
        ILogger<ScanService> logger)
    {
        _scanRepository = scanRepository;
        _anomalieRepository = anomalieRepository;
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    #region SEC-013: Validation QR Code

    public async Task<ScanValidationResult> ValidateScanAsync(ScanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validation scan QR à la barrière {BarriereId} par {Agent}", 
                request.BarriereId, request.AgentLogin);

            // 1. Valider le format du QR Code
            var qrValidation = await _qrCodeService.ValidateQRCodeAsync(request.QRCodeData);
            if (!qrValidation.IsValid)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.Invalid,
                    $"QR Code invalide: {qrValidation.ErrorMessage}");
            }

            // 2. Décoder le QR Code pour obtenir les infos du bon
            var decodedInfo = _qrCodeService.DecodeHash(request.QRCodeData);
            if (decodedInfo == null)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.Invalid,
                    "Impossible de décoder le QR Code");
            }

            // 3. Récupérer le bon correspondant
            var (bon, typeBon) = await GetBonAsync(decodedInfo.BonId, decodedInfo.BonType, cancellationToken);
            if (bon == null)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.NotFound,
                    $"Bon {decodedInfo.BonType}-{decodedInfo.NumeroReference} non trouvé");
            }

            // 4. Vérifier l'expiration
            if (bon.DateExpiration < DateTime.Now)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.Expired,
                    $"Le bon a expiré le {bon.DateExpiration:dd/MM/yyyy}");
            }

            // 5. SEC-015: Vérifier l'unicité (pas de doublon)
            var canScan = await CheckUniciteAsync(bon.IdBon, typeBon, request.BarriereId, cancellationToken);
            if (!canScan)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.AlreadyUsed,
                    "Ce bon a déjà été scanné à cette barrière");
            }

            // 6. SEC-014: Vérifier l'itinéraire
            var itineraireResult = await VerifierItineraireAsync(bon.IdBon, typeBon, request.BarriereId, cancellationToken);
            
            // Créer le résultat de validation
            var result = new ScanValidationResult
            {
                IsValid = true,
                StatutScan = StatutScanValues.Valid,
                Message = itineraireResult.BarriereAutorisee 
                    ? "Scan validé - Barrière conforme à l'itinéraire" 
                    : "Scan validé - ATTENTION: Barrière hors itinéraire prévu",
                BonId = bon.IdBon,
                TypeBon = typeBon,
                NumeroReference = bon.NumeroReference,
                Provenance = bon.Provenance,
                Destination = bon.Destination,
                NombreMateriels = bon.Quantite,
                DateExpiration = bon.DateExpiration
            };

            // SEC-016: Détecter les anomalies potentielles
            var scanInfo = new ScanInfo
            {
                BonId = bon.IdBon,
                TypeBon = typeBon,
                NumeroReference = bon.NumeroReference,
                BarriereId = request.BarriereId,
                DateHeureScan = DateTime.Now,
                BonTrouve = true,
                BonExpire = false,
                DejaScanne = false,
                BarriereAutorisee = itineraireResult.BarriereAutorisee
            };

            var anomalies = await DetecterAnomaliesAsync(scanInfo, cancellationToken);
            result.Anomalies = anomalies;

            if (!itineraireResult.BarriereAutorisee)
            {
                // Ajouter un warning mais ne pas invalider le scan
                result.Message = "Scan validé avec warning - Barrière hors itinéraire prévu";
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du scan");
            return ScanValidationResult.Invalid(StatutScanValues.Invalid, "Erreur lors de la validation");
        }
    }

    #endregion

    #region SEC-012: Traitement Scan

    public async Task<ScanProcessResult> ProcessScanAsync(ScanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Traitement scan QR à la barrière {BarriereId} par {Agent}", 
                request.BarriereId, request.AgentLogin);

            // 1. Valider le scan d'abord
            var validation = await ValidateScanAsync(request, cancellationToken);

            // 2. Décoder le QR Code
            var decodedInfo = _qrCodeService.DecodeHash(request.QRCodeData);
            
            // 3. Créer l'événement de scan
            var scan = new ScanEvenement
            {
                DateHeureScan = DateTime.Now,
                StatutScan = validation.StatutScan,
                TypeMouvement = DeterminerTypeMouvement(validation.TypeBon),
                BonId = validation.BonId,
                TypeBon = validation.TypeBon,
                NumeroReferenceBon = validation.NumeroReference,
                BarriereId = request.BarriereId,
                AgentLogin = request.AgentLogin,
                AgentNom = request.AgentNom,
                QRCodeData = request.QRCodeData,
                QRCodeHash = decodedInfo?.HashedCode,
                Message = validation.Message,
                Observations = request.Observations,
                AnomalieSignalee = validation.Anomalies.Any()
            };

            // 4. Sauvegarder le scan
            await _scanRepository.CreateScanAsync(scan, cancellationToken);

            // 5. SEC-017: Créer les anomalies détectées
            var anomaliesCreees = new List<Anomalie>();
            foreach (var anomalieDetectee in validation.Anomalies)
            {
                var anomalie = new Anomalie
                {
                    TypeAnomalie = anomalieDetectee.TypeAnomalie,
                    NiveauGravite = anomalieDetectee.NiveauGravite,
                    DateSignalement = DateTime.Now,
                    Description = anomalieDetectee.Description,
                    BonId = validation.BonId,
                    TypeBon = validation.TypeBon,
                    NumeroReferenceBon = validation.NumeroReference,
                    ScanId = scan.IdScan,
                    BarriereId = request.BarriereId,
                    SignalePar = request.AgentLogin,
                    SignaleParNom = request.AgentNom
                };
                await _anomalieRepository.CreateAsync(anomalie, cancellationToken);
                anomaliesCreees.Add(anomalie);
            }

            // 6. SEC-018: Générer preuve de passage si valide
            PreuvePassage? preuve = null;
            if (validation.IsValid)
            {
                preuve = await GenererPreuvePassageAsync(scan.IdScan, cancellationToken);
                
                // Mettre à jour l'itinéraire avec la date de passage effective
                await UpdateItinerairePassageAsync(validation.BonId!.Value, validation.TypeBon!, 
                    request.BarriereId, cancellationToken);
            }

            // 7. Créer l'entrée dans l'historique
            await CreateHistoriqueScanAsync(scan, validation, cancellationToken);

            return new ScanProcessResult
            {
                Success = true,
                Message = validation.Message,
                Scan = scan,
                Preuve = preuve,
                AnomaliesCreees = anomaliesCreees
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du traitement du scan");
            return ScanProcessResult.Failed($"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region SEC-014: Vérification Itinéraire

    public async Task<ItineraireVerificationResult> VerifierItineraireAsync(
        int bonId, 
        string typeBon, 
        int barriereId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = new ItineraireVerificationResult();

            if (typeBon == "BEM")
            {
                // Vérifier dans ItinerairePrevu pour BonEntree
                var itineraire = await _scanRepository.GetItinerairePrevuAsync(bonId, barriereId, cancellationToken);

                if (itineraire != null)
                {
                    result.BarriereAutorisee = true;
                    result.OrdreAttendu = itineraire.OrdrePassage;
                    result.Message = $"Barrière conforme - Ordre de passage: {itineraire.OrdrePassage}";
                }
                else
                {
                    result.BarriereAutorisee = false;
                    result.Message = "Barrière non prévue dans l'itinéraire du bon d'entrée";
                }
            }
            else if (typeBon == "BSM" || typeBon == "BSE" || typeBon == "BSI")
            {
                // Vérifier dans ItineraireSortie pour BonSortie
                var itineraire = await _scanRepository.GetItineraireSortieAsync(bonId, barriereId, cancellationToken);

                if (itineraire != null)
                {
                    result.BarriereAutorisee = true;
                    result.OrdreAttendu = itineraire.OrdrePassage;
                    result.Message = $"Barrière conforme - Ordre de passage: {itineraire.OrdrePassage}";
                }
                else
                {
                    result.BarriereAutorisee = false;
                    result.Message = "Barrière non prévue dans l'itinéraire du bon de sortie";
                }
            }

            // Vérifier l'ordre de passage
            result.OrdreCorrect = await VerifierOrdrePassageAsync(bonId, typeBon, barriereId, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de l'itinéraire");
            return new ItineraireVerificationResult
            {
                BarriereAutorisee = false,
                Message = "Erreur lors de la vérification"
            };
        }
    }

    public async Task<bool> VerifierOrdrePassageAsync(
        int bonId, 
        string typeBon, 
        int barriereId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Récupérer les scans précédents pour ce bon
            var scansPrecedents = await _scanRepository.GetScansByBonAsync(bonId, typeBon, cancellationToken);
            
            if (!scansPrecedents.Any())
                return true; // Premier scan, ordre correct par défaut

            // Vérifier que la barrière actuelle a un ordre supérieur aux barrières déjà scannées
            // Cette logique dépend de la structure de l'itinéraire
            return true; // Simplification - à enrichir selon les besoins métier
        }
        catch
        {
            return true; // En cas d'erreur, ne pas bloquer
        }
    }

    #endregion

    #region SEC-015: Unicité Scan

    public async Task<bool> CheckUniciteAsync(
        int bonId, 
        string typeBon, 
        int barriereId, 
        CancellationToken cancellationToken = default)
    {
        // Un bon ne peut être scanné qu'une seule fois de manière valide à chaque barrière
        return !await _scanRepository.HasBeenScannedAtBarriereAsync(bonId, typeBon, barriereId, cancellationToken);
    }

    #endregion

    #region SEC-016: Détection Anomalies

    public async Task<IReadOnlyList<AnomalieDetectee>> DetecterAnomaliesAsync(
        ScanInfo scanInfo, 
        CancellationToken cancellationToken = default)
    {
        var anomalies = new List<AnomalieDetectee>();

        // 1. Barrière hors itinéraire
        if (!scanInfo.BarriereAutorisee)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.BarriereNonAutorisee,
                NiveauGravite = NiveauGraviteValues.Eleve,
                Description = $"Passage à une barrière non prévue dans l'itinéraire du bon {scanInfo.NumeroReference}"
            });
        }

        // 2. Document expiré
        if (scanInfo.BonExpire)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.BonExpire,
                NiveauGravite = NiveauGraviteValues.Critique,
                Description = $"Le bon {scanInfo.NumeroReference} a expiré"
            });
        }

        // 3. Document non trouvé
        if (!scanInfo.BonTrouve)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.BonNonTrouve,
                NiveauGravite = NiveauGraviteValues.Critique,
                Description = "Aucun bon correspondant au QR Code scanné"
            });
        }

        // 4. Déjà scanné
        if (scanInfo.DejaScanne)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.QRCodeDejaScan,
                NiveauGravite = NiveauGraviteValues.Eleve,
                Description = $"Le bon {scanInfo.NumeroReference} a déjà été scanné à cette barrière"
            });
        }

        return anomalies;
    }

    #endregion

    #region SEC-018: Preuve de Passage

    public async Task<PreuvePassage> GenererPreuvePassageAsync(
        int scanId, 
        CancellationToken cancellationToken = default)
    {
        var scan = await _scanRepository.GetByIdAsync(scanId, cancellationToken);
        if (scan == null)
            throw new InvalidOperationException($"Scan {scanId} non trouvé");

        var preuve = new PreuvePassage
        {
            ScanId = scanId,
            NumeroPreuve = GenerateNumeroPreuve(scan),
            DateHeure = scan.DateHeureScan,
            NomBarriere = scan.Barriere?.NomBarriere ?? "Inconnue",
            TypeMouvement = scan.TypeMouvement,
            NumeroReferenceBon = scan.NumeroReferenceBon ?? "",
            TypeBon = scan.TypeBon ?? "",
            AgentLogin = scan.AgentLogin,
            AgentNom = scan.AgentNom,
            Observations = scan.Observations
        };

        // Récupérer provenance/destination depuis le bon
        if (scan.BonId.HasValue && !string.IsNullOrEmpty(scan.TypeBon))
        {
            var (bon, _) = await GetBonAsync(scan.BonId.Value, scan.TypeBon, cancellationToken);
            if (bon != null)
            {
                preuve.Provenance = bon.Provenance;
                preuve.Destination = bon.Destination;
                preuve.NombreMateriels = bon.Quantite;
            }
        }

        // Générer un QR Code pour la preuve
        var preuveData = $"PREUVE|{preuve.NumeroPreuve}|{preuve.DateHeure:yyyyMMddHHmmss}|{scanId}";
        preuve.QRCodeBase64 = _qrCodeService.GenerateQRCode(preuveData);

        return preuve;
    }

    private static string GenerateNumeroPreuve(ScanEvenement scan)
    {
        return $"PRV-{scan.DateHeureScan:yyyyMMdd}-{scan.IdScan:D6}";
    }

    public async Task<PreuvePassage?> GetPreuvePassageAsync(
        int scanId, 
        CancellationToken cancellationToken = default)
    {
        var scan = await _scanRepository.GetByIdAsync(scanId, cancellationToken);
        if (scan == null || scan.StatutScan != StatutScanValues.Valid)
            return null;

        // Régénérer la preuve à partir des données du scan
        return await GenererPreuvePassageAsync(scanId, cancellationToken);
    }

    #endregion

    #region Historique

    public async Task<ScanHistoryResult> GetScanHistoryAsync(
        ScanHistoryFilter filter, 
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _scanRepository.SearchAsync(
            filter.SearchTerm,
            filter.BarriereId,
            filter.StatutScan,
            filter.TypeMouvement,
            filter.AgentLogin,
            filter.DateDebut,
            filter.DateFin,
            filter.Skip,
            filter.Take,
            cancellationToken);

        return new ScanHistoryResult
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = filter.Take,
            CurrentPage = filter.Skip / filter.Take + 1
        };
    }

    public async Task<IReadOnlyList<ScanEvenement>> GetScansByBonAsync(
        int bonId, 
        string typeBon, 
        CancellationToken cancellationToken = default)
    {
        return await _scanRepository.GetScansByBonAsync(bonId, typeBon, cancellationToken);
    }

    public async Task<IReadOnlyList<ScanEvenement>> GetScansByBarriereAsync(
        int barriereId,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default)
    {
        return await _scanRepository.GetScansParBarriereAsync(barriereId, dateDebut, dateFin, cancellationToken);
    }

    #endregion

    #region Statistiques

    public async Task<ScanStats> GetStatistiquesAsync(
        DateTime dateDebut,
        DateTime dateFin,
        int? barriereId = null,
        CancellationToken cancellationToken = default)
    {
        var parStatut = await _scanRepository.GetScanCountByStatutAsync(dateDebut, dateFin, barriereId, cancellationToken);

        var stats = new ScanStats
        {
            ParStatut = parStatut,
            TotalScans = parStatut.Values.Sum(),
            ScansValides = parStatut.GetValueOrDefault(StatutScanValues.Valid, 0),
            ScansInvalides = parStatut.Values.Sum() - parStatut.GetValueOrDefault(StatutScanValues.Valid, 0)
        };

        // Scans avec anomalies
        var scansAnomalies = await _scanRepository.GetScansAvecAnomaliesAsync(dateDebut, dateFin, cancellationToken);
        stats.ScansAvecAnomalies = scansAnomalies.Count;

        // Taux de validation
        stats.TauxValidation = stats.TotalScans > 0 
            ? (double)stats.ScansValides / stats.TotalScans * 100 
            : 0;

        return stats;
    }

    #endregion

    #region Méthodes Privées

    private async Task<(BonInfo? Bon, string TypeBon)> GetBonAsync(int bonId, string typeBon, CancellationToken cancellationToken)
    {
        var scanBonInfo = await _scanRepository.GetBonInfoAsync(bonId, typeBon, cancellationToken);

        if (scanBonInfo != null)
        {
            return (new BonInfo
            {
                IdBon = scanBonInfo.IdBon,
                NumeroReference = scanBonInfo.NumeroReference,
                Provenance = scanBonInfo.Provenance,
                Destination = scanBonInfo.Destination,
                DateExpiration = scanBonInfo.DateExpiration,
                Quantite = scanBonInfo.Quantite
            }, typeBon == "BEM" ? "BEM" : typeBon);
        }

        return (null, typeBon);
    }

    private static string DeterminerTypeMouvement(string? typeBon)
    {
        return typeBon switch
        {
            "BEM" => TypeMouvementValues.Entree,
            _ => TypeMouvementValues.Sortie
        };
    }

    private async Task UpdateItinerairePassageAsync(int bonId, string typeBon, int barriereId, CancellationToken cancellationToken)
    {
        try
        {
            if (typeBon == "BEM")
            {
                // Pas de DatePassageEffective dans ItinerairePrevu pour BEM
            }
            else
            {
                await _scanRepository.UpdateItineraireSortiePassageAsync(bonId, barriereId, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de mettre à jour l'itinéraire de passage");
        }
    }

    private async Task CreateHistoriqueScanAsync(ScanEvenement scan, ScanValidationResult validation, CancellationToken cancellationToken)
    {
        try
        {
            var historique = new HistoriqueScan
            {
                ScanId = scan.IdScan,
                DateHeureMouvement = scan.DateHeureScan,
                TypeMouvement = scan.TypeMouvement,
                TypeBon = scan.TypeBon ?? "N/A",
                NumeroReferenceBon = scan.NumeroReferenceBon ?? "N/A",
                CodeBarriere = scan.Barriere?.CodeBarriere ?? "N/A",
                NomBarriere = scan.Barriere?.NomBarriere,
                Provenance = validation.Provenance,
                Destination = validation.Destination,
                NombreMateriels = validation.NombreMateriels,
                StatutScan = scan.StatutScan,
                PassageAutorise = validation.IsValid,
                AgentLogin = scan.AgentLogin,
                AgentNom = scan.AgentNom,
                Observations = scan.Observations,
                AnomalieSignalee = scan.AnomalieSignalee,
                NombreAnomalies = validation.Anomalies.Count
            };

            await _scanRepository.CreateHistoriqueScanAsync(historique, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de créer l'entrée d'historique pour le scan {ScanId}", scan.IdScan);
        }
    }

    #endregion
}

/// <summary>
/// DTO interne pour les informations de bon
/// </summary>
internal class BonInfo
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DateExpiration { get; set; }
    public int Quantite { get; set; }
}
