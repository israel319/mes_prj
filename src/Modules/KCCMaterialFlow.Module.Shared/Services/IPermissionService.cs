using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des permissions.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Récupère toutes les permissions actives
    /// </summary>
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une permission par son identifiant
    /// </summary>
    Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une permission par son code
    /// </summary>
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les permissions par catégorie
    /// </summary>
    Task<IReadOnlyList<Permission>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les permissions d'un rôle
    /// </summary>
    Task<IReadOnlyList<Permission>> GetPermissionsForRoleAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attribue une permission à un rôle
    /// </summary>
    Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retire une permission d'un rôle
    /// </summary>
    Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour toutes les permissions d'un rôle
    /// </summary>
    Task UpdateRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un rôle a une permission spécifique
    /// </summary>
    Task<bool> RoleHasPermissionAsync(int roleId, string codePermission, CancellationToken cancellationToken = default);
}
