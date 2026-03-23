using KCCMaterialFlow.Module.BonEntree.Services;
using KCCMaterialFlow.Module.BonSortie.Services;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// INT-003: Service de recherche unifiée cross-module
/// Permet de rechercher dans tous les types de bons depuis une seule interface
/// </summary>
public interface IUnifiedSearchService
{
    /// <summary>
    /// Recherche globale dans tous les modules
    /// </summary>
    Task<UnifiedSearchResult> SearchAsync(string query, int maxResults = 20, CancellationToken cancellationToken = default);
}

public class UnifiedSearchService : IUnifiedSearchService
{
    private readonly IBonEntreeService _bonEntreeService;
    private readonly IBonSortieService _bonSortieService;

    public UnifiedSearchService(
        IBonEntreeService bonEntreeService,
        IBonSortieService bonSortieService)
    {
        _bonEntreeService = bonEntreeService;
        _bonSortieService = bonSortieService;
    }

    public async Task<UnifiedSearchResult> SearchAsync(string query, int maxResults = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return new UnifiedSearchResult();
        }

        var results = new List<SearchResultItem>();

        // Rechercher en parallèle
        var bonEntreeTask = SearchBonsEntreeAsync(query, maxResults / 2, cancellationToken);
        var bonSortieTask = SearchBonsSortieAsync(query, maxResults / 2, cancellationToken);

        await Task.WhenAll(bonEntreeTask, bonSortieTask);

        results.AddRange(bonEntreeTask.Result);
        results.AddRange(bonSortieTask.Result);

        return new UnifiedSearchResult
        {
            Query = query,
            TotalResults = results.Count,
            Items = results.OrderByDescending(r => r.Score).Take(maxResults).ToList()
        };
    }

    private async Task<List<SearchResultItem>> SearchBonsEntreeAsync(string query, int maxResults, CancellationToken cancellationToken)
    {
        var results = new List<SearchResultItem>();

        try
        {
            var filter = new KCCMaterialFlow.Module.BonEntree.Repositories.BonEntreeFilter
            {
                SearchTerm = query,
                Take = maxResults
            };

            var searchResult = await _bonEntreeService.GetListAsync(filter, cancellationToken);

            foreach (var bon in searchResult.Items)
            {
                var score = CalculateScore(query, bon.NumeroReference, bon.NomCompagnie, bon.Description);
                results.Add(new SearchResultItem
                {
                    Id = bon.IdBon,
                    Type = "BEM",
                    TypeLabel = "Bon d'Entrée",
                    Reference = bon.NumeroReference,
                    Title = bon.NomCompagnie ?? "N/A",
                    Subtitle = bon.ReasonOnSite,
                    Statut = bon.StatutActuel,
                    Date = bon.DateCreation,
                    Url = $"/bon-entree/{bon.IdBon}",
                    Icon = "inventory_2",
                    Color = "#1976d2",
                    Score = score
                });
            }
        }
        catch
        {
            // Ignorer les erreurs de recherche
        }

        return results;
    }

    private async Task<List<SearchResultItem>> SearchBonsSortieAsync(string query, int maxResults, CancellationToken cancellationToken)
    {
        var results = new List<SearchResultItem>();

        try
        {
            var filter = new KCCMaterialFlow.Module.BonSortie.Services.BonSortieFilter
            {
                SearchTerm = query,
                Take = maxResults
            };

            var searchResult = await _bonSortieService.GetListAsync(filter, cancellationToken);

            foreach (var bon in searchResult.Items)
            {
                var isExterne = bon is KCCMaterialFlow.Module.BonSortie.Entities.BonSortieExterne;
                var score = CalculateScore(query, bon.NumeroReference, bon.NomDemandeur, bon.MotifSortie);
                
                results.Add(new SearchResultItem
                {
                    Id = bon.IdBon,
                    Type = isExterne ? "BSM" : "BSI",
                    TypeLabel = isExterne ? "Bon de Sortie Externe" : "Bon de Sortie Interne",
                    Reference = bon.NumeroReference,
                    Title = bon.NomDemandeur ?? "N/A",
                    Subtitle = bon.MotifSortie,
                    Statut = bon.StatutActuel,
                    Date = bon.DateCreation,
                    Url = $"/bon-sortie/{bon.IdBon}",
                    Icon = isExterne ? "logout" : "swap_horiz",
                    Color = "#444",
                    Score = score
                });
            }
        }
        catch
        {
            // Ignorer les erreurs de recherche
        }

        return results;
    }

    private static int CalculateScore(string query, params string?[] fields)
    {
        var score = 0;
        var queryLower = query.ToLowerInvariant();

        foreach (var field in fields.Where(f => !string.IsNullOrEmpty(f)))
        {
            var fieldLower = field!.ToLowerInvariant();
            
            // Match exact = score élevé
            if (fieldLower == queryLower)
            {
                score += 100;
            }
            // Commence par la query
            else if (fieldLower.StartsWith(queryLower))
            {
                score += 50;
            }
            // Contient la query
            else if (fieldLower.Contains(queryLower))
            {
                score += 20;
            }
        }

        return score;
    }
}

public class UnifiedSearchResult
{
    public string Query { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public List<SearchResultItem> Items { get; set; } = new();
}

public class SearchResultItem
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TypeLabel { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = "description";
    public string Color { get; set; } = "#444";
    public int Score { get; set; }
}
