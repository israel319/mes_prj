using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service pour la gestion des motifs/raisons d'entrée structurés.
/// Utilise IDbContextFactory + IMemoryCache (même pattern que CategorieSortieService).
/// </summary>
public class RaisonEntreeService : IRaisonEntreeService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;

    private const string CacheAllActiveKey = "RaisonsEntree_AllActive";
    private const string CacheMappingPrefix = "RaisonEntree_Mappings_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public RaisonEntreeService(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory, IMemoryCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RaisonEntree>> GetAllActiveAsync()
    {
        return await _cache.GetOrCreateAsync(CacheAllActiveKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return (IReadOnlyList<RaisonEntree>)await context.RaisonsEntree
                .AsNoTracking()
                .Where(r => r.EstActif)
                .OrderBy(r => r.OrdreAffichage)
                .ThenBy(r => r.Nom)
                .ToListAsync();
        }) ?? [];
    }

    /// <inheritdoc />
    public async Task<RaisonEntree?> GetByIdAsync(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.RaisonsEntree
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <inheritdoc />
    public async Task<RaisonEntree?> GetByCodeAsync(string code)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.RaisonsEntree
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Code != null && r.Code == code);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieByRaisonEntreeIdAsync(int raisonEntreeId)
    {
        var cacheKey = $"{CacheMappingPrefix}{raisonEntreeId}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            return (IReadOnlyList<RaisonSortie>)await context.RaisonEntreeRaisonsSortie
                .AsNoTracking()
                .Where(m => m.RaisonEntreeId == raisonEntreeId)
                .Include(m => m.RaisonSortie)
                    .ThenInclude(rs => rs.Categorie)
                .OrderBy(m => m.OrdreAffichage)
                .Select(m => m.RaisonSortie)
                .Where(rs => rs.EstActif)
                .ToListAsync();
        }) ?? [];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieByRaisonEntreeCodeAsync(string code)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var raisonEntree = await context.RaisonsEntree
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Code != null && r.Code == code);

        if (raisonEntree == null)
            return [];

        return await GetRaisonsSortieByRaisonEntreeIdAsync(raisonEntree.Id);
    }
}
