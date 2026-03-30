using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Securite;

/// <summary>
/// SEC-012: Service métier pour les scans de sécurité.
/// Implémente la validation QR, vérification d'itinéraire, unicité et détection d'anomalies.
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

            var qrValidation = await _qrCodeService.ValidateQRCodeAsync(request.QRCodeData);
            if (!qrValidation.IsValid)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.Invalid,
                    $"QR Code invalide: {qrValidation.ErrorMessage}");
            }

            var decodedInfo = _qrCodeService.DecodeHash(request.QRCodeData);
            if (decodedInfo == null)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.Invalid,
                    "Impossible de décoder le QR Code");
            }

            var (bon, typeBon) = await GetBonAsync(decodedInfo.BonId, decodedInfo.BonType, cancellationToken);
            if (bon == null)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.NotFound,
                    $"Bon {decodedInfo.BonType}-{decodedInfo.NumeroReference} non trouvé");
            }

            if (bon.DateExpiration < DateTime.Now)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.Expired,
                    $"Le bon a expiré le {bon.DateExpiration:dd/MM/yyyy}");
            }

            var canScan = await CheckUniciteAsync(bon.IdBon, typeBon, request.BarriereId, cancellationToken);
            if (!canScan)
            {
                return ScanValidationResult.Invalid(
                    StatutScanValues.AlreadyUsed,
                    "Ce bon a déjà été scanné à cette barrière");
            }

            var itineraireResult = await VerifierItineraireAsync(bon.IdBon, typeBon, request.BarriereId, cancellationToken);

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

            var validation = await ValidateScanAsync(request, cancellationToken);

            var decodedInfo = _qrCodeService.DecodeHash(request.QRCodeData);

            var scan = new ScanEvenement(
                barriereId: request.BarriereId,
                agentLogin: request.AgentLogin,
                typeMouvement: DeterminerTypeMouvement(validation.TypeBon),
                qrCodeData: request.QRCodeData,
                qrCodeHash: decodedInfo?.HashedCode,
                bonId: validation.BonId,
                typeBon: validation.TypeBon,
                numeroReferenceBon: validation.NumeroReference,
                agentNom: request.AgentNom);

            scan.StatutScan = validation.StatutScan;
            if (validation.Message != null) scan.MarquerValide(validation.Message);
            if (validation.Anomalies.Count > 0) scan.MarquerAnomalie();
            if (request.Observations != null) scan.SetObservations(request.Observations);

            await _scanRepository.CreateScanAsync(scan, cancellationToken);

            var anomaliesCreees = new List<Anomalie>();
            foreach (var anomalieDetectee in validation.Anomalies)
            {
                var anomalie = new Anomalie(
                    typeAnomalie: anomalieDetectee.TypeAnomalie,
                    description: anomalieDetectee.Description,
                    signalePar: request.AgentLogin,
                    niveauGravite: anomalieDetectee.NiveauGravite,
                    bonId: validation.BonId,
                    typeBon: validation.TypeBon,
                    numeroReferenceBon: validation.NumeroReference,
                    scanId: scan.Id,
                    barriereId: request.BarriereId,
                    signaleParNom: request.AgentNom);
                await _anomalieRepository.CreateAsync(anomalie, cancellationToken);
                anomaliesCreees.Add(anomalie);
            }

            PreuvePassage? preuve = null;
            if (validation.IsValid)
            {
                preuve = await GenererPreuvePassageAsync(scan.Id, cancellationToken);

                await UpdateItinerairePassageAsync(validation.BonId!.Value, validation.TypeBon!,
                    request.BarriereId, cancellationToken);
            }

            await CreateHistoriqueScanAsync(scan, validation, cancellationToken);

            return new ScanProcessResult
            {
                Success = true,
                Message = validation.Message ?? string.Empty,
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
            else if (typeBon is "BSM" or "BSE" or "BSI")
            {
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
            var scansPrecedents = await _scanRepository.GetScansByBonAsync(bonId, typeBon, cancellationToken);

            if (!scansPrecedents.Any())
                return true;

            return true; // Simplification - à enrichir selon les besoins métier
        }
        catch
        {
            return true;
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
        return !await _scanRepository.HasBeenScannedAtBarriereAsync(bonId, typeBon, barriereId, cancellationToken);
    }

    #endregion

    #region SEC-016: Détection Anomalies

    public Task<IReadOnlyList<AnomalieDetectee>> DetecterAnomaliesAsync(
        ScanInfo scanInfo,
        CancellationToken cancellationToken = default)
    {
        var anomalies = new List<AnomalieDetectee>();

        if (!scanInfo.BarriereAutorisee)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.BarriereNonAutorisee,
                NiveauGravite = NiveauGraviteValues.Eleve,
                Description = $"Passage à une barrière non prévue dans l'itinéraire du bon {scanInfo.NumeroReference}"
            });
        }

        if (scanInfo.BonExpire)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.BonExpire,
                NiveauGravite = NiveauGraviteValues.Critique,
                Description = $"Le bon {scanInfo.NumeroReference} a expiré"
            });
        }

        if (!scanInfo.BonTrouve)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.BonNonTrouve,
                NiveauGravite = NiveauGraviteValues.Critique,
                Description = "Aucun bon correspondant au QR Code scanné"
            });
        }

        if (scanInfo.DejaScanne)
        {
            anomalies.Add(new AnomalieDetectee
            {
                TypeAnomalie = TypeAnomalieValues.QRCodeDejaScan,
                NiveauGravite = NiveauGraviteValues.Eleve,
                Description = $"Le bon {scanInfo.NumeroReference} a déjà été scanné à cette barrière"
            });
        }

        return Task.FromResult<IReadOnlyList<AnomalieDetectee>>(anomalies);
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

        var preuveData = $"PREUVE|{preuve.NumeroPreuve}|{preuve.DateHeure:yyyyMMddHHmmss}|{scanId}";
        preuve.QRCodeBase64 = _qrCodeService.GenerateQRCode(preuveData);

        return preuve;
    }

    private static string GenerateNumeroPreuve(ScanEvenement scan)
    {
        return $"PRV-{scan.DateHeureScan:yyyyMMdd}-{scan.Id:D6}";
    }

    public async Task<PreuvePassage?> GetPreuvePassageAsync(
        int scanId,
        CancellationToken cancellationToken = default)
    {
        var scan = await _scanRepository.GetByIdAsync(scanId, cancellationToken);
        if (scan == null || scan.StatutScan != StatutScanValues.Valid)
            return null;

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

        var scansAnomalies = await _scanRepository.GetScansAvecAnomaliesAsync(dateDebut, dateFin, cancellationToken);
        stats.ScansAvecAnomalies = scansAnomalies.Count;

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
            var historique = new HistoriqueScan(
                scanId: scan.Id,
                typeMouvement: scan.TypeMouvement,
                typeBon: scan.TypeBon ?? "N/A",
                numeroReferenceBon: scan.NumeroReferenceBon ?? "N/A",
                codeBarriere: scan.Barriere?.CodeBarriere ?? "N/A",
                agentLogin: scan.AgentLogin,
                statutScan: scan.StatutScan,
                passageAutorise: validation.IsValid,
                nomBarriere: scan.Barriere?.NomBarriere,
                agentNom: scan.AgentNom,
                provenance: validation.Provenance,
                destination: validation.Destination,
                nombreMateriels: validation.NombreMateriels,
                observations: scan.Observations);

            await _scanRepository.CreateHistoriqueScanAsync(historique, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de créer l'entrée d'historique pour le scan {ScanId}", scan.Id);
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
