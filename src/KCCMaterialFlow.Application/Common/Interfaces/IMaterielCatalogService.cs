namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>Suggestion catalogue matériel pour autocomplete (déduit des matériels existants).</summary>
public sealed class MaterielSuggestion
{
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string? DepartementCode { get; set; }
    public string DisplayLabel =>
        $"{CodeProduitSerial} — {Designation}"
        + (string.IsNullOrWhiteSpace(DepartementCode) ? "" : $" [{DepartementCode}]");
}

/// <summary>Recherche dans le catalogue dérivé des T_Materiels (déduplication par code+désignation).</summary>
public interface IMaterielCatalogService
{
    /// <summary>
    /// Recherche distinct (code, désignation). Si <paramref name="departementCode"/> est fourni,
    /// retourne uniquement les matériels dont DepartementCode == departementCode OU est null
    /// (matériels multi-département).
    /// </summary>
    Task<IReadOnlyList<MaterielSuggestion>> SearchAsync(
        string? fragment, string? departementCode, int maxResults = 20, CancellationToken ct = default);
}
