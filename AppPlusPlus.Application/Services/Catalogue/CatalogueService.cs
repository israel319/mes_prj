using AppPlusPlus.Domain.Entities.Catalogue;
using AppPlusPlus.Application.Interfaces.Repositories;
using StockEntity = AppPlusPlus.Domain.Entities.Stock.Stock;

namespace AppPlusPlus.Application.Services.Catalogue;

public class CatalogueService : ICatalogueService
{
    private readonly ICatalogueRepository _catalogueRepo;
    private readonly ILookupRepository _lookupRepo;
    private readonly IStockRepository _stockRepo;

    public CatalogueService(
        ICatalogueRepository catalogueRepo,
        ILookupRepository lookupRepo,
        IStockRepository stockRepo)
    {
        _catalogueRepo = catalogueRepo;
        _lookupRepo = lookupRepo;
        _stockRepo = stockRepo;
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

    public async Task<bool> ArticleExistsAsync(string articleCode)
        => await _catalogueRepo.ArticleExistsAsync(articleCode);

    public async Task<bool> ArticleDescriptionExistsAsync(string description)
        => await _catalogueRepo.ArticleDescriptionExistsAsync(description);

    public async Task CreateArticleWithStocksAsync(Article article, List<int> localisationIds)
    {
        // 1) Create the article
        await _catalogueRepo.AddAsync(article);

        // 2) Create a Stock entry for each localisation (Qte=0, Seuil=0)
        foreach (var locId in localisationIds)
        {
            var stock = new StockEntity
            {
                IdArticle = article.IdArticle,
                IdLocalisation = locId,
                Qte = 0,
                Seuil = 0,
                DateSys = DateOnly.FromDateTime(DateTime.Today),
                UserLogin = article.User
            };
            await _stockRepo.AddAsync(stock);
        }
    }

    // Localisation (read)
    public async Task<Domain.Entities.Administration.Localisation?> GetLocalisationByIdAsync(int id)
        => await _lookupRepo.GetLocalisationByIdAsync(id);
}
