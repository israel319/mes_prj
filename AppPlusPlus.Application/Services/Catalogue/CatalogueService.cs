using AppPlusPlus.Domain.Entities.Catalogue;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Catalogue;

public class CatalogueService : ICatalogueService
{
    private readonly ICatalogueRepository _catalogueRepo;
    private readonly ILookupRepository _lookupRepo;

    public CatalogueService(ICatalogueRepository catalogueRepo, ILookupRepository lookupRepo)
    {
        _catalogueRepo = catalogueRepo;
        _lookupRepo = lookupRepo;
    }

    // Articles
    public async Task<List<Article>> GetArticlesAsync()
        => await _catalogueRepo.GetAllAsync();

    // Categories
    public async Task<List<ArticleCategory>> GetArticleCategoriesAsync()
        => await _catalogueRepo.GetAllCategoriesAsync();

    public async Task AddCategoryAsync(ArticleCategory category)
        => await _catalogueRepo.AddCategoryAsync(category);

    public async Task UpdateCategoryAsync(ArticleCategory category)
        => await _catalogueRepo.UpdateCategoryAsync(category);

    public async Task DeleteCategoryAsync(ArticleCategory category)
        => await _catalogueRepo.DeleteCategoryAsync(category);

    // Types
    public async Task<List<ArticleType>> GetArticleTypesAsync()
        => await _catalogueRepo.GetAllTypesAsync();

    public async Task AddTypeAsync(ArticleType type)
        => await _catalogueRepo.AddTypeAsync(type);

    public async Task UpdateTypeAsync(ArticleType type)
        => await _catalogueRepo.UpdateTypeAsync(type);

    public async Task DeleteTypeAsync(ArticleType type)
        => await _catalogueRepo.DeleteTypeAsync(type);

    // Marques
    public async Task<List<ArticleMarque>> GetArticleMarquesAsync()
        => await _catalogueRepo.GetAllMarquesAsync();

    public async Task AddMarqueAsync(ArticleMarque marque)
        => await _catalogueRepo.AddMarqueAsync(marque);

    public async Task UpdateMarqueAsync(ArticleMarque marque)
        => await _catalogueRepo.UpdateMarqueAsync(marque);

    public async Task DeleteMarqueAsync(ArticleMarque marque)
        => await _catalogueRepo.DeleteMarqueAsync(marque);

    // Mesures
    public async Task<List<Mesure>> GetMesuresAsync()
        => await _catalogueRepo.GetAllMesuresAsync();

    public async Task AddMesureAsync(Mesure mesure)
        => await _catalogueRepo.AddMesureAsync(mesure);

    public async Task UpdateMesureAsync(Mesure mesure)
        => await _catalogueRepo.UpdateMesureAsync(mesure);

    public async Task DeleteMesureAsync(Mesure mesure)
        => await _catalogueRepo.DeleteMesureAsync(mesure);

    // Localisations
    public async Task AddLocalisationAsync(Domain.Entities.Administration.Localisation localisation)
        => await _lookupRepo.AddLocalisationAsync(localisation);

    public async Task UpdateLocalisationAsync(Domain.Entities.Administration.Localisation localisation)
        => await _lookupRepo.UpdateLocalisationAsync(localisation);

    public async Task DeleteLocalisationAsync(Domain.Entities.Administration.Localisation localisation)
        => await _lookupRepo.DeleteLocalisationAsync(localisation);

    // Article CRUD
    public async Task<Article?> GetArticleByIdAsync(string id)
        => await _catalogueRepo.GetByArticleIdAsync(id);

    public async Task AddArticleAsync(Article article)
        => await _catalogueRepo.AddAsync(article);

    public async Task UpdateArticleAsync(Article article)
        => await _catalogueRepo.UpdateAsync(article);

    public async Task DeleteArticleAsync(string articleId)
    {
        var article = await _catalogueRepo.GetByArticleIdAsync(articleId);
        if (article != null)
            await _catalogueRepo.DeleteAsync(article);
    }

    // Localisation (read)
    public async Task<Domain.Entities.Administration.Localisation?> GetLocalisationByIdAsync(int id)
        => await _lookupRepo.GetLocalisationByIdAsync(id);
}
