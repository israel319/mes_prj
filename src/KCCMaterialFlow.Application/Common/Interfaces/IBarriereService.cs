using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IBarriereService
{
    Task<IReadOnlyList<Barriere>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Barriere?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Barriere?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Barriere>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Barriere>> SearchByLocalisationAsync(string localisation, CancellationToken cancellationToken = default);
    Task<Barriere> CreateAsync(Barriere barriere, CancellationToken cancellationToken = default);
    Task<Barriere> UpdateAsync(Barriere barriere, CancellationToken cancellationToken = default);
    void InvalidateCache();
}
