using AppPlusPlus.Domain.Interfaces.Repositories;
using StockEntity = AppPlusPlus.Domain.Entities.Stock.Stock;

namespace AppPlusPlus.Application.Services.Stock;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepo;

    public StockService(IStockRepository stockRepo)
    {
        _stockRepo = stockRepo;
    }

    public async Task<List<StockEntity>> GetStocksByLocalisationsAsync(List<int> localisationIds)
        => await _stockRepo.GetByLocalisationIdsAsync(localisationIds);

    public async Task<List<StockEntity>> GetAllStocksAsync()
        => await _stockRepo.GetAllAsync();

    public async Task<List<StockEntity>> GetLowStockArticlesAsync(List<int> localisationIds)
        => await _stockRepo.GetLowStockAsync(localisationIds);
}
