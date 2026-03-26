using AppPlusPlus.Domain.Entities.Catalogue;

namespace AppPlusPlus.Domain.Interfaces.Repositories;

public interface ICatalogueRepository : IRepository<Article>
{
    Task<Article?> GetByArticleIdAsync(string articleId);
    Task<List<Article>> GetArticlesWithStockAsync(List<int> localisationIds);
    Task<List<ArticleType>> GetAllTypesAsync();
    Task<List<ArticleMarque>> GetAllMarquesAsync();
    Task<List<ArticleCategory>> GetAllCategoriesAsync();
    Task<List<Mesure>> GetAllMesuresAsync();

    // CRUD for reference data
    Task AddCategoryAsync(ArticleCategory category);
    Task UpdateCategoryAsync(ArticleCategory category);
    Task DeleteCategoryAsync(ArticleCategory category);
    Task AddTypeAsync(ArticleType type);
    Task UpdateTypeAsync(ArticleType type);
    Task DeleteTypeAsync(ArticleType type);
    Task AddMarqueAsync(ArticleMarque marque);
    Task UpdateMarqueAsync(ArticleMarque marque);
    Task DeleteMarqueAsync(ArticleMarque marque);
    Task AddMesureAsync(Mesure mesure);
    Task UpdateMesureAsync(Mesure mesure);
    Task DeleteMesureAsync(Mesure mesure);
}
