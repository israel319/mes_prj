namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Types de bons disponibles dans le système
/// </summary>
public enum BonType
{
    /// <summary>
    /// Bon d'Entrée Matériel (BEM) - Pour l'entrée de matériel sur le site
    /// </summary>
    Entree = 1,

    /// <summary>
    /// Bon de Sortie Interne - Matériel KCC sortant du site
    /// </summary>
    SortieInterne = 2,

    /// <summary>
    /// Bon de Sortie Externe - Matériel contracteur retournant (lié à un bon d'entrée)
    /// </summary>
    SortieExterne = 3
}
