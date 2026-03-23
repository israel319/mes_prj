using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des types de matériel.
/// </summary>
public interface ITypeMaterielService
{
    /// <summary>
    /// Récupère tous les types de matériel actifs
    /// </summary>
    Task<IReadOnlyList<TypeMateriel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un type de matériel par son identifiant
    /// </summary>
    Task<TypeMateriel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un type de matériel par son code
    /// </summary>
    Task<TypeMateriel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les types de matériel par catégorie
    /// </summary>
    Task<IReadOnlyList<TypeMateriel>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau type de matériel
    /// </summary>
    Task<TypeMateriel> CreateAsync(TypeMateriel typeMateriel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un type de matériel existant
    /// </summary>
    Task<TypeMateriel> UpdateAsync(TypeMateriel typeMateriel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un type de matériel (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un code de type existe déjà
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la liste des catégories distinctes
    /// </summary>
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
