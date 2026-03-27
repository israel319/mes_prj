using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Domain.Entities.Catalogue;
using AppPlusPlus.Application.Interfaces.Repositories;

namespace AppPlusPlus.Infrastructure.Persistence.Repositories;

public class CatalogueRepository : RepositoryBase<Article>, ICatalogueRepository
{
    public CatalogueRepository(IDbContextFactory<AppDbContext> dbFactory) : base(dbFactory) { }

    public async Task<Article?> GetByArticleIdAsync(string articleId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Articles.FirstOrDefaultAsync(a => a.IdArticle == articleId);
    }

    public async Task<bool> ArticleExistsAsync(string articleCode)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Articles.AnyAsync(a => a.IdArticle == articleCode);
    }

    public async Task<bool> ArticleDescriptionExistsAsync(string description)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Articles.AnyAsync(a => a.Description == description);
    }

    public async Task<List<Article>> GetArticlesWithStockAsync(List<int> localisationIds)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Articles.ToListAsync();
    }

    public async Task<List<ArticleType>> GetAllTypesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.ArticleTypes.ToListAsync();
    }

    public async Task<List<ArticleMarque>> GetAllMarquesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.ArticleMarques.ToListAsync();
    }

    public async Task<List<ArticleCategory>> GetAllCategoriesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.ArticleCategories.ToListAsync();
    }

    public async Task<List<Mesure>> GetAllMesuresAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Mesures.ToListAsync();
    }

    // CRUD for reference data
    public async Task AddCategoryAsync(ArticleCategory category)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleCategories.Add(category);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(ArticleCategory category)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleCategories.Update(category);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(ArticleCategory category)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleCategories.Remove(category);
        await ctx.SaveChangesAsync();
    }

    public async Task AddTypeAsync(ArticleType type)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleTypes.Add(type);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateTypeAsync(ArticleType type)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleTypes.Update(type);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteTypeAsync(ArticleType type)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleTypes.Remove(type);
        await ctx.SaveChangesAsync();
    }

    public async Task AddMarqueAsync(ArticleMarque marque)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleMarques.Add(marque);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateMarqueAsync(ArticleMarque marque)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleMarques.Update(marque);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteMarqueAsync(ArticleMarque marque)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.ArticleMarques.Remove(marque);
        await ctx.SaveChangesAsync();
    }

    public async Task AddMesureAsync(Mesure mesure)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.Mesures.Add(mesure);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateMesureAsync(Mesure mesure)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.Mesures.Update(mesure);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteMesureAsync(Mesure mesure)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        ctx.Mesures.Remove(mesure);
        await ctx.SaveChangesAsync();
    }
}
