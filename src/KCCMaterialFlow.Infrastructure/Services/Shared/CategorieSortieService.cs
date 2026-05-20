using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service pour la gestion des catÃ©gories et raisons de sortie.
/// ImplÃ©mentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class CategorieSortieService : ICategorieSortieService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;

    private const string CacheCategoriesKey = "CategoriesSortie_All";
    private const string CacheCategoriesWithRaisonsKey = "CategoriesSortie_WithRaisons";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public CategorieSortieService(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory, IMemoryCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CategorieSortie>> GetAllCategoriesAsync()
    {
        return await _cache.GetOrCreateAsync(CacheCategoriesKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Set<CategorieSortie>()
                .Where(c => c.EstActif)
                .OrderBy(c => c.OrdreAffichage)
                .ThenBy(c => c.Nom)
                .ToListAsync();
        }) ?? [];
    }

    /// <inheritdoc />
    public async Task<CategorieSortie?> GetCategorieByIdAsync(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<CategorieSortie>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc />
    public async Task<CategorieSortie?> GetCategorieByCodeAsync(string code)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<CategorieSortie>()
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieIdAsync(int categorieId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<RaisonSortie>()
            .Where(r => r.CategorieId == categorieId && r.EstActif)
            .OrderBy(r => r.OrdreAffichage)
            .ThenBy(r => r.Nom)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieCodeAsync(string categorieCode)
    {
        var categorie = await GetCategorieByCodeAsync(categorieCode);
        if (categorie == null)
            return [];

        return await GetRaisonsByCategorieIdAsync(categorie.Id);
    }

    /// <inheritdoc />
    public async Task<RaisonSortie?> GetRaisonByIdAsync(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<RaisonSortie>()
            .Include(r => r.Categorie)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CategorieSortie>> GetAllCategoriesWithRaisonsAsync()
    {
        return await _cache.GetOrCreateAsync(CacheCategoriesWithRaisonsKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Set<CategorieSortie>()
                .Include(c => c.Raisons.Where(r => r.EstActif).OrderBy(r => r.OrdreAffichage))
                .Where(c => c.EstActif)
                .OrderBy(c => c.OrdreAffichage)
                .ThenBy(c => c.Nom)
                .ToListAsync();
        }) ?? [];
    }
}
