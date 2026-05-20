using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface ICategorieSortieService
{
    Task<IEnumerable<CategorieSortie>> GetAllCategoriesAsync();
    Task<CategorieSortie?> GetCategorieByIdAsync(int id);
    Task<CategorieSortie?> GetCategorieByCodeAsync(string code);
    Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieIdAsync(int categorieId);
    Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieCodeAsync(string categorieCode);
    Task<RaisonSortie?> GetRaisonByIdAsync(int id);
    Task<IEnumerable<CategorieSortie>> GetAllCategoriesWithRaisonsAsync();
}
