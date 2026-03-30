using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IParametreSystemeService
{
    Task<IReadOnlyList<ParametreSysteme>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ParametreSysteme?> GetByKeyAsync(string cle, CancellationToken cancellationToken = default);
    Task<string?> GetValueAsync(string cle, CancellationToken cancellationToken = default);
    Task<int> GetIntValueAsync(string cle, int defaultValue = 0, CancellationToken cancellationToken = default);
    Task<bool> GetBoolValueAsync(string cle, bool defaultValue = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ParametreSysteme>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default);
    Task<bool> UpdateValueAsync(string cle, string valeur, string modifieParLogin, CancellationToken cancellationToken = default);
    Task<ParametreSysteme> CreateAsync(ParametreSysteme parametre, CancellationToken cancellationToken = default);
    Task<ParametreSysteme> UpdateAsync(ParametreSysteme parametre, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    void InvalidateCache();
}
