namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Représente un élément du menu de navigation
/// </summary>
public class NavMenuItem
{
    /// <summary>
    /// Identifiant unique de l'élément de menu
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Libellé affiché dans le menu
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// URL de destination (href)
    /// </summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>
    /// Classe d'icône (ex: "fa fa-plus", "oi oi-list")
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant du parent pour les sous-menus (null si racine)
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Ordre d'affichage dans le menu
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Rôles autorisés à voir cet élément (vide = tous)
    /// </summary>
    public List<string> AllowedRoles { get; set; } = new();

    /// <summary>
    /// Indique si l'élément est visible
    /// </summary>
    public bool IsVisible { get; set; } = true;
}
