using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IRoleService
{
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default);
    Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetRolesForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> AssignRoleToUserAsync(int userId, int roleId, string assignedByLogin, CancellationToken cancellationToken = default);
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
}
