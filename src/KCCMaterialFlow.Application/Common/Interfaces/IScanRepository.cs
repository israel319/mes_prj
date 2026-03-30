using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Interface du repository pour les événements de scan.
/// SEC-007: Fournit l'accès aux données ScanEvenement avec méthodes spécialisées.
/// </summary>
public interface IScanRepository
{
    #region CRUD de base

    /// <summary>
    /// Récupère un scan par son ID
    /// </summary>
    Task<ScanEvenement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau scan dans la base de données
    /// </summary>
    Task<ScanEvenement> CreateScanAsync(ScanEvenement scan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un scan existant
    /// </summary>
    Task UpdateAsync(ScanEvenement scan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un scan
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region Requêtes par Bon

    /// <summary>
    /// Récupère tous les scans pour un bon donné
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="typeBon">Type de bon (BEM ou BSM)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<IReadOnlyList<ScanEvenement>> GetScansByBonAsync(
        int bonId,
        string typeBon,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un bon a déjà été scanné à une barrière spécifique
    /// </summary>
    Task<bool> HasBeenScannedAtBarriereAsync(
        int bonId,
        string typeBon,
        int barriereId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le dernier scan d'un bon
    /// </summary>
    Task<ScanEvenement?> GetLastScanForBonAsync(
        int bonId,
        string typeBon,
        CancellationToken cancellationToken = default);

    #endregion

    #region Requêtes par Barrière

    /// <summary>
    /// Récupère tous les scans effectués à une barrière donnée
    /// </summary>
    /// <param name="barriereId">Identifiant de la barrière</param>
    /// <param name="dateDebut">Date de début (optionnel)</param>
    /// <param name="dateFin">Date de fin (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<IReadOnlyList<ScanEvenement>> GetScansParBarriereAsync(
        int barriereId,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les scans d'une barrière avec pagination
    /// </summary>
    Task<(IReadOnlyList<ScanEvenement> Items, int TotalCount)> GetScansParBarrierePagedAsync(
        int barriereId,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        string? statutScan = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    #endregion

    #region Requêtes par Agent

    /// <summary>
    /// Récupère tous les scans effectués par un agent
    /// </summary>
    /// <param name="agentLogin">Login de l'agent</param>
    /// <param name="dateDebut">Date de début (optionnel)</param>
    /// <param name="dateFin">Date de fin (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<IReadOnlyList<ScanEvenement>> GetScansByAgentAsync(
        string agentLogin,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Recherche et Statistiques

    /// <summary>
    /// Recherche des scans avec filtres multiples et pagination
    /// </summary>
    Task<(IReadOnlyList<ScanEvenement> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        int? barriereId = null,
        string? statutScan = null,
        string? typeMouvement = null,
        string? agentLogin = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le nombre de scans par statut pour une période donnée
    /// </summary>
    Task<Dictionary<string, int>> GetScanCountByStatutAsync(
        DateTime dateDebut,
        DateTime dateFin,
        int? barriereId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les scans avec anomalies
    /// </summary>
    Task<IReadOnlyList<ScanEvenement>> GetScansAvecAnomaliesAsync(
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Itinéraire & Historique (Clean Architecture)

    /// <summary>
    /// Vérifie si une barrière est dans l'itinéraire prévu d'un BonEntree.
    /// </summary>
    Task<ItineraireInfo?> GetItinerairePrevuAsync(int bonId, int barriereId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si une barrière est dans l'itinéraire d'un BonSortie.
    /// </summary>
    Task<ItineraireInfo?> GetItineraireSortieAsync(int bonSortieId, int barriereId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les informations de base d'un bon (BEM ou BSM) pour les opérations de scan.
    /// </summary>
    Task<ScanBonInfo?> GetBonInfoAsync(int bonId, string typeBon, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour le passage d'un itinéraire de sortie après un scan valide.
    /// </summary>
    Task UpdateItineraireSortiePassageAsync(int bonSortieId, int barriereId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée une entrée d'historique de scan.
    /// </summary>
    Task CreateHistoriqueScanAsync(HistoriqueScan historique, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// DTO pour les informations d'itinéraire retournées par le repository.
/// </summary>
public class ItineraireInfo
{
    public int OrdrePassage { get; set; }
}

/// <summary>
/// DTO pour les informations de base d'un bon lors d'opérations de scan.
/// </summary>
public class ScanBonInfo
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DateExpiration { get; set; }
    public int Quantite { get; set; }
}
