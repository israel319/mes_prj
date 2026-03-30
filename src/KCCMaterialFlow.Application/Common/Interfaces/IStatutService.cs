using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IStatutService
{
    Task<IReadOnlyList<Statut>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Statut?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Statut?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Statut>> GetByTypeBonAsync(string typeBon, CancellationToken cancellationToken = default);
    Task<Statut> CreateAsync(Statut statut, CancellationToken cancellationToken = default);
    Task<Statut> UpdateAsync(Statut statut, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Statut>> GetNextStatusesAsync(int statutId, CancellationToken cancellationToken = default);
}
