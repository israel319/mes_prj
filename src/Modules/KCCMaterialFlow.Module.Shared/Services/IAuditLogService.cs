using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de journalisation d'audit.
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Enregistre une action dans le journal d'audit
    /// </summary>
    Task LogAsync(
        string utilisateurLogin,
        string? utilisateurNom,
        string typeAction,
        string categorie,
        string description,
        string? entiteId = null,
        string? entiteType = null,
        string? details = null,
        string? ancienneValeur = null,
        string? nouvelleValeur = null,
        string resultat = "Succes",
        string niveau = "Info",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les entrées d'audit avec pagination
    /// </summary>
    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        string? utilisateurLogin = null,
        string? typeAction = null,
        string? categorie = null,
        string? niveau = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les entrées d'audit pour une entité spécifique
    /// </summary>
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entiteType,
        string entiteId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les entrées d'audit d'un utilisateur
    /// </summary>
    Task<IReadOnlyList<AuditLog>> GetByUserAsync(
        string utilisateurLogin,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les types d'actions distincts
    /// </summary>
    Task<IReadOnlyList<string>> GetActionTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les catégories distinctes
    /// </summary>
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les statistiques d'audit pour une période
    /// </summary>
    Task<AuditStats> GetStatsAsync(
        DateTime dateDebut,
        DateTime dateFin,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO pour les statistiques d'audit
/// </summary>
public class AuditStats
{
    public int TotalActions { get; set; }
    public int TotalUsers { get; set; }
    public int TotalErrors { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public Dictionary<string, int> ActionsByCategory { get; set; } = new();
    public Dictionary<string, int> ActionsByUser { get; set; } = new();
    public List<(DateTime Date, int Count)> ActionsPerDay { get; set; } = new();
}
