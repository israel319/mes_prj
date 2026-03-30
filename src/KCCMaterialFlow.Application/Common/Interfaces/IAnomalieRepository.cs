using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Interface du repository pour les anomalies.
/// SEC-009: Fournit l'accès aux données Anomalie avec méthodes spécialisées.
/// </summary>
public interface IAnomalieRepository
{
    #region CRUD de base

    /// <summary>
    /// Récupère une anomalie par son ID
    /// </summary>
    Task<Anomalie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée une nouvelle anomalie dans la base de données
    /// </summary>
    Task<Anomalie> CreateAsync(Anomalie anomalie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une anomalie existante
    /// </summary>
    Task UpdateAsync(Anomalie anomalie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime une anomalie
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region Requêtes par Bon

    /// <summary>
    /// Récupère toutes les anomalies pour un bon donné
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="typeBon">Type de bon (BEM ou BSM)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<IReadOnlyList<Anomalie>> GetByBonAsync(
        int bonId,
        string typeBon,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un bon a des anomalies non traitées
    /// </summary>
    Task<bool> HasAnomaliesNonTraiteesAsync(
        int bonId,
        string typeBon,
        CancellationToken cancellationToken = default);

    #endregion

    #region Anomalies Non Traitées

    /// <summary>
    /// Récupère toutes les anomalies non traitées
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<IReadOnlyList<Anomalie>> GetNonTraiteesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les anomalies non traitées avec filtres
    /// </summary>
    /// <param name="niveauGravite">Filtrer par niveau de gravité (optionnel)</param>
    /// <param name="typeAnomalie">Filtrer par type d'anomalie (optionnel)</param>
    /// <param name="barriereId">Filtrer par barrière (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<IReadOnlyList<Anomalie>> GetNonTraiteesAsync(
        string? niveauGravite = null,
        string? typeAnomalie = null,
        int? barriereId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le nombre d'anomalies non traitées par niveau de gravité
    /// </summary>
    Task<Dictionary<string, int>> GetCountByGraviteAsync(
        bool nonTraiteesSeulementNot = true,
        CancellationToken cancellationToken = default);

    #endregion

    #region Requêtes par Scan

    /// <summary>
    /// Récupère toutes les anomalies associées à un scan
    /// </summary>
    Task<IReadOnlyList<Anomalie>> GetByScanAsync(
        int scanId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Traitement des Anomalies

    /// <summary>
    /// Marque une anomalie comme traitée
    /// </summary>
    /// <param name="id">Identifiant de l'anomalie</param>
    /// <param name="traitePar">Login de l'utilisateur qui traite</param>
    /// <param name="resolution">Description de la résolution</param>
    /// <param name="actionsCorrectives">Actions correctives prises (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task MarquerCommeTraiteeAsync(
        int id,
        string traitePar,
        string resolution,
        string? actionsCorrectives = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Recherche et Statistiques

    /// <summary>
    /// Recherche des anomalies avec filtres multiples et pagination
    /// </summary>
    Task<(IReadOnlyList<Anomalie> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? typeAnomalie = null,
        string? niveauGravite = null,
        bool? estTraitee = null,
        int? barriereId = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les statistiques d'anomalies pour une période
    /// </summary>
    Task<AnomalieStats> GetStatistiquesAsync(
        DateTime dateDebut,
        DateTime dateFin,
        CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Statistiques des anomalies pour le tableau de bord
/// </summary>
public class AnomalieStats
{
    public int TotalAnomalies { get; set; }
    public int AnomaliesTraitees { get; set; }
    public int AnomaliesNonTraitees { get; set; }
    public int AnomaliesCritiques { get; set; }
    public Dictionary<string, int> ParType { get; set; } = new();
    public Dictionary<string, int> ParGravite { get; set; } = new();
    public double TauxResolution { get; set; }
    public TimeSpan? DelaiMoyenResolution { get; set; }
}
