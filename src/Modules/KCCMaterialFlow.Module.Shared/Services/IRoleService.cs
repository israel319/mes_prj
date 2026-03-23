using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des rôles.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Récupère tous les rôles actifs
    /// </summary>
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un rôle par son identifiant
    /// </summary>
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un rôle par son code
    /// </summary>
    Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau rôle
    /// </summary>
    Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un rôle existant
    /// </summary>
    Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un rôle (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les rôles d'un utilisateur
    /// </summary>
    Task<IReadOnlyList<Role>> GetRolesForUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attribue un rôle à un utilisateur
    /// </summary>
    Task<bool> AssignRoleToUserAsync(int userId, int roleId, string assignedByLogin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retire un rôle d'un utilisateur
    /// </summary>
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un code de rôle existe déjà
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
}
