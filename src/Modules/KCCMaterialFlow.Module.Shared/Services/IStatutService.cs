using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des statuts.
/// </summary>
public interface IStatutService
{
    /// <summary>
    /// Récupère tous les statuts actifs
    /// </summary>
    Task<IReadOnlyList<Statut>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un statut par son identifiant
    /// </summary>
    Task<Statut?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un statut par son code
    /// </summary>
    Task<Statut?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les statuts pour un type de bon spécifique
    /// </summary>
    Task<IReadOnlyList<Statut>> GetByTypeBonAsync(string typeBon, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau statut
    /// </summary>
    Task<Statut> CreateAsync(Statut statut, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un statut existant
    /// </summary>
    Task<Statut> UpdateAsync(Statut statut, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un statut (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un code de statut existe déjà
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les statuts suivants possibles pour un statut donné
    /// </summary>
    Task<IReadOnlyList<Statut>> GetNextStatusesAsync(int statutId, CancellationToken cancellationToken = default);
}
