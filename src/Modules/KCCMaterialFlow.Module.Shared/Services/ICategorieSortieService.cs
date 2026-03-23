using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des catégories et raisons de sortie
/// </summary>
public interface ICategorieSortieService
{
    /// <summary>
    /// Récupère toutes les catégories de sortie actives
    /// </summary>
    Task<IEnumerable<CategorieSortie>> GetAllCategoriesAsync();

    /// <summary>
    /// Récupère une catégorie par son identifiant
    /// </summary>
    Task<CategorieSortie?> GetCategorieByIdAsync(int id);

    /// <summary>
    /// Récupère une catégorie par son code
    /// </summary>
    Task<CategorieSortie?> GetCategorieByCodeAsync(string code);

    /// <summary>
    /// Récupère les raisons de sortie pour une catégorie donnée
    /// </summary>
    Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieIdAsync(int categorieId);

    /// <summary>
    /// Récupère les raisons de sortie pour un code de catégorie
    /// </summary>
    Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieCodeAsync(string categorieCode);

    /// <summary>
    /// Récupère une raison par son identifiant
    /// </summary>
    Task<RaisonSortie?> GetRaisonByIdAsync(int id);

    /// <summary>
    /// Récupère toutes les catégories avec leurs raisons
    /// </summary>
    Task<IEnumerable<CategorieSortie>> GetAllCategoriesWithRaisonsAsync();
}
