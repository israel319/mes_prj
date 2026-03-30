using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IAuditLogService
{
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

    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entiteType,
        string entiteId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLog>> GetByUserAsync(
        string utilisateurLogin,
        int limit = 100,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetActionTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<AuditStats> GetStatsAsync(
        DateTime dateDebut,
        DateTime dateFin,
        CancellationToken cancellationToken = default);
}

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
