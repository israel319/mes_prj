using KCCMaterialFlow.Module.Securite.Entities;

namespace KCCMaterialFlow.Module.Securite.Services;

/// <summary>
/// SEC-019: Interface du service métier pour les anomalies.
/// Gère la création, le traitement et le suivi des anomalies.
/// </summary>
public interface IAnomalieService
{
    #region CRUD Operations

    /// <summary>
    /// SEC-017: Crée une nouvelle anomalie avec notification Investigation
    /// </summary>
    /// <param name="request">Données de l'anomalie</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Anomalie créée</returns>
    Task<AnomalieResult> CreateAsync(CreateAnomalieRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une anomalie par son ID
    /// </summary>
    Task<Anomalie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une anomalie
    /// </summary>
    Task<AnomalieResult> UpdateAsync(UpdateAnomalieRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Traitement (SEC-020)

    /// <summary>
    /// Marque une anomalie comme traitée
    /// </summary>
    /// <param name="id">ID de l'anomalie</param>
    /// <param name="resolution">Description de la résolution</param>
    /// <param name="actionsCorrectives">Actions correctives prises (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<AnomalieResult> MarkAsTraiteeAsync(
        int id, 
        string resolution, 
        string? actionsCorrectives = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Réouvre une anomalie précédemment traitée
    /// </summary>
    Task<AnomalieResult> ReopenAsync(int id, string motif, CancellationToken cancellationToken = default);

    #endregion

    #region Requêtes

    /// <summary>
    /// Récupère les anomalies non traitées
    /// </summary>
    Task<IReadOnlyList<Anomalie>> GetNonTraiteesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les anomalies critiques non traitées
    /// </summary>
    Task<IReadOnlyList<Anomalie>> GetCritiquesNonTraiteesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les anomalies d'un bon
    /// </summary>
    Task<IReadOnlyList<Anomalie>> GetByBonAsync(int bonId, string typeBon, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les anomalies d'un scan
    /// </summary>
    Task<IReadOnlyList<Anomalie>> GetByScanAsync(int scanId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche des anomalies avec filtres
    /// </summary>
    Task<AnomalieSearchResult> SearchAsync(AnomalieFilter filter, CancellationToken cancellationToken = default);

    #endregion

    #region Dashboard et Statistiques

    /// <summary>
    /// Récupère les données du tableau de bord anomalies
    /// </summary>
    Task<AnomalieDashboard> GetDashboardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les statistiques d'anomalies pour une période
    /// </summary>
    Task<Repositories.AnomalieStats> GetStatistiquesAsync(
        DateTime dateDebut,
        DateTime dateFin,
        CancellationToken cancellationToken = default);

    #endregion

    #region Notifications

    /// <summary>
    /// SEC-017: Envoie une notification pour une anomalie critique
    /// </summary>
    Task NotifyInvestigationAsync(Anomalie anomalie, CancellationToken cancellationToken = default);

    #endregion
}

#region DTOs et Résultats

/// <summary>
/// Requête de création d'anomalie
/// </summary>
public class CreateAnomalieRequest
{
    public string TypeAnomalie { get; set; } = string.Empty;
    public string NiveauGravite { get; set; } = NiveauGraviteValues.Moyen;
    public string Description { get; set; } = string.Empty;
    public int? BonId { get; set; }
    public string? TypeBon { get; set; }
    public string? NumeroReferenceBon { get; set; }
    public int? ScanId { get; set; }
    public int? BarriereId { get; set; }
}

/// <summary>
/// Requête de mise à jour d'anomalie
/// </summary>
public class UpdateAnomalieRequest
{
    public int IdAnomalie { get; set; }
    public string? NiveauGravite { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Résultat d'opération sur anomalie
/// </summary>
public class AnomalieResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Anomalie? Anomalie { get; set; }

    public static AnomalieResult Ok(Anomalie anomalie, string message = "Opération réussie") => new()
    {
        Success = true,
        Message = message,
        Anomalie = anomalie
    };

    public static AnomalieResult Failed(string message) => new()
    {
        Success = false,
        Message = message
    };
}

/// <summary>
/// Filtre de recherche d'anomalies
/// </summary>
public class AnomalieFilter
{
    public string? SearchTerm { get; set; }
    public string? TypeAnomalie { get; set; }
    public string? NiveauGravite { get; set; }
    public bool? EstTraitee { get; set; }
    public int? BarriereId { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}

/// <summary>
/// Résultat de recherche d'anomalies
/// </summary>
public class AnomalieSearchResult
{
    public IReadOnlyList<Anomalie> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
}

/// <summary>
/// Données du tableau de bord anomalies
/// </summary>
public class AnomalieDashboard
{
    /// <summary>
    /// Nombre total d'anomalies non traitées
    /// </summary>
    public int TotalNonTraitees { get; set; }

    /// <summary>
    /// Nombre d'anomalies critiques non traitées
    /// </summary>
    public int CritiquesNonTraitees { get; set; }

    /// <summary>
    /// Nombre d'anomalies élevées non traitées
    /// </summary>
    public int EleveesNonTraitees { get; set; }

    /// <summary>
    /// Anomalies récentes (dernières 24h)
    /// </summary>
    public IReadOnlyList<Anomalie> AnomaliesRecentes { get; set; } = [];

    /// <summary>
    /// Répartition par type
    /// </summary>
    public Dictionary<string, int> ParType { get; set; } = new();

    /// <summary>
    /// Répartition par gravité
    /// </summary>
    public Dictionary<string, int> ParGravite { get; set; } = new();

    /// <summary>
    /// Taux de résolution (sur 30 jours)
    /// </summary>
    public double TauxResolution { get; set; }

    /// <summary>
    /// Délai moyen de résolution
    /// </summary>
    public TimeSpan? DelaiMoyenResolution { get; set; }
}

#endregion
