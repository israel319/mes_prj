using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface ITypeMaterielService
{
    Task<IReadOnlyList<TypeMaterielEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TypeMaterielEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TypeMaterielEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TypeMaterielEntity>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default);
    Task<TypeMaterielEntity> CreateAsync(TypeMaterielEntity typeMateriel, CancellationToken cancellationToken = default);
    Task<TypeMaterielEntity> UpdateAsync(TypeMaterielEntity typeMateriel, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
