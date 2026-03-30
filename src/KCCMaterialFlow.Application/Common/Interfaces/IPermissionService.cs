using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsForRoleAsync(int roleId, CancellationToken cancellationToken = default);
    Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task UpdateRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default);
    Task<bool> RoleHasPermissionAsync(int roleId, string codePermission, CancellationToken cancellationToken = default);
}
