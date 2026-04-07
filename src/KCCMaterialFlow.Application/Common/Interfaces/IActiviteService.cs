using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IActiviteService
{
    Task<IReadOnlyList<Activite>> GetAllActivitesAsync(CancellationToken cancellationToken = default);
    Task<Activite?> GetActiviteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Activite?> GetActiviteByCodeAsync(string codeActivite, CancellationToken cancellationToken = default);
    Task<Dictionary<string, List<Activite>>> GetActivitesGroupedByModuleAsync(CancellationToken cancellationToken = default);
    IReadOnlyList<string> GetDefaultActiviteCodesForRole(string roleCode);
}
