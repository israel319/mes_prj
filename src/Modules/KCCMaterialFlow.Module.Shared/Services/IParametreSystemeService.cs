using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des paramètres système.
/// </summary>
public interface IParametreSystemeService
{
    /// <summary>
    /// Récupère tous les paramètres visibles
    /// </summary>
    Task<IReadOnlyList<ParametreSysteme>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un paramètre par sa clé
    /// </summary>
    Task<ParametreSysteme?> GetByKeyAsync(string cle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la valeur d'un paramètre sous forme de chaîne
    /// </summary>
    Task<string?> GetValueAsync(string cle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la valeur d'un paramètre sous forme d'entier
    /// </summary>
    Task<int> GetIntValueAsync(string cle, int defaultValue = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la valeur d'un paramètre sous forme de booléen
    /// </summary>
    Task<bool> GetBoolValueAsync(string cle, bool defaultValue = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les paramètres par catégorie
    /// </summary>
    Task<IReadOnlyList<ParametreSysteme>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour la valeur d'un paramètre
    /// </summary>
    Task<bool> UpdateValueAsync(string cle, string valeur, string modifieParLogin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau paramètre
    /// </summary>
    Task<ParametreSysteme> CreateAsync(ParametreSysteme parametre, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un paramètre existant
    /// </summary>
    Task<ParametreSysteme> UpdateAsync(ParametreSysteme parametre, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un paramètre (si non système)
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la liste des catégories distinctes
    /// </summary>
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le cache des paramètres
    /// </summary>
    void InvalidateCache();
}
