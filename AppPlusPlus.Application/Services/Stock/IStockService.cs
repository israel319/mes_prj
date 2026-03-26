using StockEntity = AppPlusPlus.Domain.Entities.Stock.Stock;

namespace AppPlusPlus.Application.Services.Stock;

public interface IStockService
{
    Task<List<StockEntity>> GetStocksByLocalisationsAsync(List<int> localisationIds);
    Task<List<StockEntity>> GetAllStocksAsync();
    Task<List<StockEntity>> GetLowStockArticlesAsync(List<int> localisationIds);
}
