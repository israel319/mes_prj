using AppPlusPlus.Domain.Interfaces.Repositories;
using StockEntity = AppPlusPlus.Domain.Entities.Stock.Stock;

namespace AppPlusPlus.Application.Services.Stock;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepo;
    private readonly ICatalogueRepository _catalogueRepo;

    public StockService(IStockRepository stockRepo, ICatalogueRepository catalogueRepo)
    {
        _stockRepo = stockRepo;
        _catalogueRepo = catalogueRepo;
    }

    public async Task<List<StockEntity>> GetStocksByLocalisationsAsync(List<int> localisationIds)
        => await _stockRepo.GetByLocalisationIdsAsync(localisationIds);

    public async Task<List<StockEntity>> GetAllStocksAsync()
        => await _stockRepo.GetAllAsync();

    public async Task<List<StockEntity>> GetLowStockArticlesAsync(List<int> localisationIds)
        => await _stockRepo.GetLowStockAsync(localisationIds);

    public async Task<List<StockEntity>> GetStocksByArticleAsync(string articleId)
        => await _stockRepo.GetByArticleIdAsync(articleId);

    public async Task DeleteArticleWithStocksAsync(string articleId)
    {
        var stocks = await _stockRepo.GetByArticleIdAsync(articleId);
        foreach (var stock in stocks)
            await _stockRepo.DeleteAsync(stock);

        var article = await _catalogueRepo.GetByArticleIdAsync(articleId);
        if (article != null)
            await _catalogueRepo.DeleteAsync(article);
    }
}
