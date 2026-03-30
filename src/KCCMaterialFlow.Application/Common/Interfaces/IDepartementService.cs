using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IDepartementService
{
    Task<IReadOnlyList<Departement>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Departement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Departement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Departement> CreateAsync(Departement departement, CancellationToken cancellationToken = default);
    Task<Departement> UpdateAsync(Departement departement, CancellationToken cancellationToken = default);
    void InvalidateCache();
}
