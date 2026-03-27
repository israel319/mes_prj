using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Domain.Entities.Stock;
using AppPlusPlus.Application.Interfaces.Repositories;

namespace AppPlusPlus.Infrastructure.Persistence.Repositories;

public class StockRepository : RepositoryBase<Stock>, IStockRepository
{
    public StockRepository(IDbContextFactory<AppDbContext> dbFactory) : base(dbFactory) { }

    public async Task<Stock?> GetByArticleAndLocalisationAsync(string articleId, int localisationId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Stocks.FirstOrDefaultAsync(s => s.IdArticle == articleId && s.IdLocalisation == localisationId);
    }

    public async Task<List<Stock>> GetByLocalisationAsync(int localisationId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Stocks.Where(s => s.IdLocalisation == localisationId)
            .Include(s => s.Article)
            .ToListAsync();
    }

    public async Task<List<Stock>> GetByLocalisationIdsAsync(List<int> localisationIds)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Stocks.Where(s => localisationIds.Contains(s.IdLocalisation))
            .Include(s => s.Article)
            .ToListAsync();
    }

    public async Task<List<Stock>> GetByArticleIdAsync(string articleId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Stocks.Where(s => s.IdArticle == articleId)
            .Include(s => s.Localisation)
            .ToListAsync();
    }

    public async Task<List<Stock>> GetLowStockAsync(List<int> localisationIds)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Stocks
            .Where(s => localisationIds.Contains(s.IdLocalisation) && s.Qte > 0 && s.Qte <= s.Seuil)
            .Include(s => s.Article)
            .Include(s => s.Localisation)
            .ToListAsync();
    }

    public async Task<List<Stock>> GetOutOfStockAsync(List<int> localisationIds)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Stocks
            .Where(s => localisationIds.Contains(s.IdLocalisation) && s.Qte <= 0)
            .Include(s => s.Article)
            .Include(s => s.Localisation)
            .ToListAsync();
    }

    public async Task AddMouvementAsync(MouvementStock mouvement)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.MouvementsStock.Add(mouvement);
        await ctx.SaveChangesAsync();
    }

    public async Task<List<MouvementStock>> GetMouvementsByArticleAsync(string articleId, int localisationId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.MouvementsStock
            .Where(m => m.IdArticle == articleId && m.IdLocalisation == localisationId)
            .OrderByDescending(m => m.DateMouvement)
            .ToListAsync();
    }

    public async Task<List<MouvementStock>> GetMouvementsByDateRangeAsync(DateTime from, DateTime to, List<int> localisationIds)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.MouvementsStock
            .Where(m => m.DateMouvement >= from && m.DateMouvement <= to && localisationIds.Contains(m.IdLocalisation))
            .Include(m => m.Article)
            .Include(m => m.Localisation)
            .OrderByDescending(m => m.DateMouvement)
            .ToListAsync();
    }
}
