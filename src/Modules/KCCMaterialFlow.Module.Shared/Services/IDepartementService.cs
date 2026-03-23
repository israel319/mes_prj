using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des départements.
/// </summary>
public interface IDepartementService
{
    /// <summary>
    /// Récupère tous les départements actifs
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste de tous les départements actifs</returns>
    Task<IReadOnlyList<Departement>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un département par son identifiant
    /// </summary>
    /// <param name="id">Identifiant du département</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Le département trouvé ou null</returns>
    Task<Departement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un département par son code
    /// </summary>
    /// <param name="code">Code du département (ex: "IT", "RH")</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Le département trouvé ou null</returns>
    Task<Departement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau département
    /// </summary>
    /// <param name="departement">Données du département</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Le département créé</returns>
    Task<Departement> CreateAsync(Departement departement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un département existant
    /// </summary>
    /// <param name="departement">Données du département</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Le département mis à jour</returns>
    Task<Departement> UpdateAsync(Departement departement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le cache des départements
    /// </summary>
    void InvalidateCache();
}
