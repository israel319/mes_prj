using KCCMaterialFlow.Module.Securite.Entities;

namespace KCCMaterialFlow.Module.Securite.Services;

/// <summary>
/// SEC-011: Interface du service métier pour les scans de sécurité.
/// Gère la validation QR, le traitement des scans et la vérification des itinéraires.
/// </summary>
public interface IScanService
{
    #region Validation QR Code (SEC-013)

    /// <summary>
    /// SEC-013: Valide un scan QR Code complet
    /// - Vérifie que le QR est valide et non expiré
    /// - Vérifie que la barrière est correcte selon l'itinéraire
    /// - Vérifie l'unicité du scan (1 scan max par barrière)
    /// </summary>
    /// <param name="request">Données du scan</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de validation avec détails</returns>
    Task<ScanValidationResult> ValidateScanAsync(ScanRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Traitement Scan (SEC-012)

    /// <summary>
    /// SEC-012: Traite un scan QR après validation
    /// - Enregistre le scan dans la base
    /// - Met à jour l'historique
    /// - Génère une preuve de passage si valide
    /// - Déclenche les anomalies si nécessaire
    /// </summary>
    /// <param name="request">Données du scan</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat du traitement avec scan créé</returns>
    Task<ScanProcessResult> ProcessScanAsync(ScanRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Vérification Itinéraire (SEC-014)

    /// <summary>
    /// SEC-014: Vérifie si une barrière fait partie de l'itinéraire prévu du bon
    /// Compare la barrière scannée vs l'itinéraire prévu
    /// </summary>
    /// <param name="bonId">ID du bon</param>
    /// <param name="typeBon">Type de bon (BEM ou BSM)</param>
    /// <param name="barriereId">ID de la barrière scannée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de vérification avec ordre de passage</returns>
    Task<ItineraireVerificationResult> VerifierItineraireAsync(
        int bonId, 
        string typeBon, 
        int barriereId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie l'ordre de passage aux barrières
    /// </summary>
    Task<bool> VerifierOrdrePassageAsync(
        int bonId, 
        string typeBon, 
        int barriereId, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Unicité Scan (SEC-015)

    /// <summary>
    /// SEC-015: Vérifie qu'un bon n'a pas déjà été scanné à cette barrière
    /// Règle: 1 scan confirmé max par barrière
    /// </summary>
    /// <param name="bonId">ID du bon</param>
    /// <param name="typeBon">Type de bon (BEM ou BSM)</param>
    /// <param name="barriereId">ID de la barrière</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>True si le scan est autorisé (pas de doublon)</returns>
    Task<bool> CheckUniciteAsync(
        int bonId, 
        string typeBon, 
        int barriereId, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Détection Anomalies (SEC-016)

    /// <summary>
    /// SEC-016: Détecte les anomalies lors d'un scan
    /// - Barrière hors itinéraire
    /// - Document expiré
    /// - Document inexistant
    /// - Scan déjà effectué
    /// </summary>
    /// <param name="scanInfo">Informations du scan en cours</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des anomalies détectées</returns>
    Task<IReadOnlyList<AnomalieDetectee>> DetecterAnomaliesAsync(
        ScanInfo scanInfo, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Preuve de Passage (SEC-018)

    /// <summary>
    /// SEC-018: Génère une preuve de passage après un scan valide
    /// Reçu généré avec informations du passage
    /// </summary>
    /// <param name="scanId">ID du scan validé</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Preuve de passage avec données pour impression/affichage</returns>
    Task<PreuvePassage> GenererPreuvePassageAsync(
        int scanId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une preuve de passage existante par ID de scan
    /// </summary>
    /// <param name="scanId">ID du scan</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Preuve de passage ou null si non trouvée</returns>
    Task<PreuvePassage?> GetPreuvePassageAsync(
        int scanId, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Historique (SEC-012)

    /// <summary>
    /// Récupère l'historique des scans avec filtres
    /// </summary>
    Task<ScanHistoryResult> GetScanHistoryAsync(
        ScanHistoryFilter filter, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les scans d'un bon spécifique
    /// </summary>
    Task<IReadOnlyList<ScanEvenement>> GetScansByBonAsync(
        int bonId, 
        string typeBon, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les scans d'une barrière pour une période
    /// </summary>
    Task<IReadOnlyList<ScanEvenement>> GetScansByBarriereAsync(
        int barriereId,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Statistiques

    /// <summary>
    /// Récupère les statistiques de scans pour le tableau de bord
    /// </summary>
    Task<ScanStats> GetStatistiquesAsync(
        DateTime dateDebut,
        DateTime dateFin,
        int? barriereId = null,
        CancellationToken cancellationToken = default);

    #endregion
}

#region DTOs et Résultats

/// <summary>
/// Requête de scan QR Code
/// </summary>
public class ScanRequest
{
    /// <summary>
    /// Données brutes du QR Code scanné
    /// </summary>
    public string QRCodeData { get; set; } = string.Empty;

    /// <summary>
    /// ID de la barrière où le scan est effectué
    /// </summary>
    public int BarriereId { get; set; }

    /// <summary>
    /// Login de l'agent de sécurité
    /// </summary>
    public string AgentLogin { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet de l'agent
    /// </summary>
    public string? AgentNom { get; set; }

    /// <summary>
    /// Observations de l'agent (optionnel)
    /// </summary>
    public string? Observations { get; set; }
}

/// <summary>
/// Informations extraites d'un scan pour analyse
/// </summary>
public class ScanInfo
{
    public int? BonId { get; set; }
    public string? TypeBon { get; set; }
    public string? NumeroReference { get; set; }
    public string? QRCodeHash { get; set; }
    public int BarriereId { get; set; }
    public DateTime DateHeureScan { get; set; }
    public bool BonTrouve { get; set; }
    public bool BonExpire { get; set; }
    public bool DejaScanne { get; set; }
    public bool BarriereAutorisee { get; set; }
}

/// <summary>
/// Résultat de validation d'un scan
/// </summary>
public class ScanValidationResult
{
    public bool IsValid { get; set; }
    public string StatutScan { get; set; } = StatutScanValues.Valid;
    public string Message { get; set; } = string.Empty;
    public int? BonId { get; set; }
    public string? TypeBon { get; set; }
    public string? NumeroReference { get; set; }
    public string? Provenance { get; set; }
    public string? Destination { get; set; }
    public int NombreMateriels { get; set; }
    public DateTime? DateExpiration { get; set; }
    public IReadOnlyList<AnomalieDetectee> Anomalies { get; set; } = [];

    public static ScanValidationResult Valid(int bonId, string typeBon, string numeroRef) => new()
    {
        IsValid = true,
        StatutScan = StatutScanValues.Valid,
        Message = "Scan validé avec succès",
        BonId = bonId,
        TypeBon = typeBon,
        NumeroReference = numeroRef
    };

    public static ScanValidationResult Invalid(string statut, string message) => new()
    {
        IsValid = false,
        StatutScan = statut,
        Message = message
    };
}

/// <summary>
/// Résultat du traitement d'un scan
/// </summary>
public class ScanProcessResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ScanEvenement? Scan { get; set; }
    public PreuvePassage? Preuve { get; set; }
    public IReadOnlyList<Anomalie> AnomaliesCreees { get; set; } = [];

    public static ScanProcessResult Ok(ScanEvenement scan, PreuvePassage? preuve = null) => new()
    {
        Success = true,
        Message = "Scan traité avec succès",
        Scan = scan,
        Preuve = preuve
    };

    public static ScanProcessResult Failed(string message) => new()
    {
        Success = false,
        Message = message
    };
}

/// <summary>
/// Résultat de vérification d'itinéraire
/// </summary>
public class ItineraireVerificationResult
{
    public bool BarriereAutorisee { get; set; }
    public bool OrdreCorrect { get; set; }
    public int? OrdreAttendu { get; set; }
    public int? OrdreActuel { get; set; }
    public string? BarrierePrecedenteAttendue { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Anomalie détectée lors d'un scan
/// </summary>
public class AnomalieDetectee
{
    public string TypeAnomalie { get; set; } = string.Empty;
    public string NiveauGravite { get; set; } = NiveauGraviteValues.Moyen;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Preuve de passage après scan valide
/// </summary>
public class PreuvePassage
{
    public int ScanId { get; set; }
    public string NumeroPreuve { get; set; } = string.Empty;
    public DateTime DateHeure { get; set; }
    public string NomBarriere { get; set; } = string.Empty;
    public string TypeMouvement { get; set; } = string.Empty;
    public string NumeroReferenceBon { get; set; } = string.Empty;
    public string TypeBon { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int NombreMateriels { get; set; }
    public string AgentLogin { get; set; } = string.Empty;
    public string? AgentNom { get; set; }
    public string? Observations { get; set; }
    public string? QRCodeBase64 { get; set; } // QR Code de la preuve pour vérification ultérieure
}

/// <summary>
/// Filtre pour l'historique des scans
/// </summary>
public class ScanHistoryFilter
{
    public int? BarriereId { get; set; }
    public string? AgentLogin { get; set; }
    public string? StatutScan { get; set; }
    public string? TypeMouvement { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? SearchTerm { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}

/// <summary>
/// Résultat de recherche d'historique
/// </summary>
public class ScanHistoryResult
{
    public IReadOnlyList<ScanEvenement> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
}

/// <summary>
/// Statistiques des scans
/// </summary>
public class ScanStats
{
    public int TotalScans { get; set; }
    public int ScansValides { get; set; }
    public int ScansInvalides { get; set; }
    public int ScansAvecAnomalies { get; set; }
    public Dictionary<string, int> ParStatut { get; set; } = new();
    public Dictionary<string, int> ParBarriere { get; set; } = new();
    public Dictionary<string, int> ParHeure { get; set; } = new();
    public double TauxValidation { get; set; }
}

#endregion
