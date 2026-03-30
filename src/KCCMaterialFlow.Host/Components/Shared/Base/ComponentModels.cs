namespace KCCMaterialFlow.Host.Components.Shared.Base;

/// <summary>
/// Arguments pour le changement de tri dans la grille
/// </summary>
public class SortChangedEventArgs
{
    /// <summary>
    /// Nom de la propriété sur laquelle trier
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Direction du tri (true = descendant)
    /// </summary>
    public bool Descending { get; set; }
}

/// <summary>
/// Arguments pour le chargement des données côté serveur
/// </summary>
public class LoadDataArgs
{
    /// <summary>
    /// Index de départ (skip)
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    /// Nombre d'éléments à charger
    /// </summary>
    public int Top { get; set; }

    /// <summary>
    /// Propriété de tri
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Tri descendant
    /// </summary>
    public bool Descending { get; set; }

    /// <summary>
    /// Filtre à appliquer
    /// </summary>
    public string? Filter { get; set; }
}

/// <summary>
/// Élément du fil d'Ariane
/// </summary>
public class BreadcrumbItem
{
    public string Text { get; set; } = string.Empty;
    public string? Url { get; set; }

    public BreadcrumbItem() { }

    public BreadcrumbItem(string text, string? url = null)
    {
        Text = text;
        Url = url;
    }
}
