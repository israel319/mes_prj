using AppPlusPlus.Domain.Entities.Administration;
using AppPlusPlus.Domain.Entities.Catalogue;

namespace AppPlusPlus.Application.Services.Catalogue;

public interface ICatalogueService
{
    // Articles
    Task<List<Article>> GetArticlesAsync();

    // Categories
    Task<List<ArticleCategory>> GetArticleCategoriesAsync();
    Task AddCategoryAsync(ArticleCategory category);
    Task UpdateCategoryAsync(ArticleCategory category);
    Task DeleteCategoryAsync(ArticleCategory category);

    // Types
    Task<List<ArticleType>> GetArticleTypesAsync();
    Task AddTypeAsync(ArticleType type);
    Task UpdateTypeAsync(ArticleType type);
    Task DeleteTypeAsync(ArticleType type);

    // Marques
    Task<List<ArticleMarque>> GetArticleMarquesAsync();
    Task AddMarqueAsync(ArticleMarque marque);
    Task UpdateMarqueAsync(ArticleMarque marque);
    Task DeleteMarqueAsync(ArticleMarque marque);

    // Mesures
    Task<List<Mesure>> GetMesuresAsync();
    Task AddMesureAsync(Mesure mesure);
    Task UpdateMesureAsync(Mesure mesure);
    Task DeleteMesureAsync(Mesure mesure);

    // Localisations
    Task AddLocalisationAsync(Domain.Entities.Administration.Localisation localisation);
    Task UpdateLocalisationAsync(Domain.Entities.Administration.Localisation localisation);
    Task DeleteLocalisationAsync(Domain.Entities.Administration.Localisation localisation);
}
