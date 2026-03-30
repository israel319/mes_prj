using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IActiviteService
{
    Task<IReadOnlyList<Activite>> GetAllActivitesAsync(CancellationToken cancellationToken = default);
    Task<Activite?> GetActiviteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Activite?> GetActiviteByCodeAsync(string codeActivite, CancellationToken cancellationToken = default);
    Task<Dictionary<string, List<Activite>>> GetActivitesGroupedByModuleAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Activite>> GetActivitesForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<int>> GetActiviteIdsForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default);
    Task UpdateUserActivitesAsync(int idUtilisateur, IEnumerable<int> activiteIds, string? attribueParLogin = null, CancellationToken cancellationToken = default);
    Task<bool> AssignActiviteToUserAsync(int idUtilisateur, int idActivite, string? attribueParLogin = null, CancellationToken cancellationToken = default);
    Task<bool> RemoveActiviteFromUserAsync(int idUtilisateur, int idActivite, CancellationToken cancellationToken = default);
    Task<bool> UserHasActiviteAsync(int idUtilisateur, string codeActivite, CancellationToken cancellationToken = default);
    Task<bool> UserHasAnyActiviteAsync(int idUtilisateur, IEnumerable<string> codeActivites, CancellationToken cancellationToken = default);
    void InvalidateUserCache(int idUtilisateur);
    Task<int> SyncActivitesFromRolesAsync(int idUtilisateur, IEnumerable<string> roleCodes, string? attribueParLogin = null, CancellationToken cancellationToken = default);
    IReadOnlyList<string> GetDefaultActiviteCodesForRole(string roleCode);
}
