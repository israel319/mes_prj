using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service pour la gestion des catégories et raisons de sortie.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
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

    /// <inheritdoc />
    public async Task<IReadOnlyList<RaisonSortie>> GetRaisonsAutoriseesByDepartementAsync(string departementCode)
    {
        var cacheKey = $"RaisonsAutorisees_{departementCode}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            // 1. Trouver le département par code OU par nom
            var dept = await context.Departements
                .FirstOrDefaultAsync(d => (d.CodeDepartement == departementCode || d.NomDepartement == departementCode) && d.EstActif);

            IReadOnlyList<RaisonSortie> raisons;

            if (dept != null)
            {
                // 2. Chercher mappings spécifiques au département
                raisons = await context.Set<DepartementRaisonSortie>()
                    .Where(m => m.DepartementId == dept.Id)
                    .Include(m => m.RaisonSortie)
                        .ThenInclude(r => r!.Categorie)
                    .OrderBy(m => m.OrdreAffichage)
                    .Select(m => m.RaisonSortie)
                    .Where(r => r != null && r.EstActif)
                    .Cast<RaisonSortie>()
                    .ToListAsync();

                if (raisons.Count > 0)
                    return raisons;
            }

            // 3. Fallback : mappings par défaut (DepartementId = NULL)
            raisons = await context.Set<DepartementRaisonSortie>()
                .Where(m => m.DepartementId == null)
                .Include(m => m.RaisonSortie)
                    .ThenInclude(r => r!.Categorie)
                .OrderBy(m => m.OrdreAffichage)
                .Select(m => m.RaisonSortie)
                .Where(r => r != null && r.EstActif)
                .Cast<RaisonSortie>()
                .ToListAsync();

            return raisons;
        }) ?? [];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RaisonSortie>> GetRaisonsAutoriseesByDepartementIdAsync(int? departementId)
    {
        var cacheKey = $"RaisonsAutorisees_Id_{departementId?.ToString() ?? "DEFAULT"}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            if (departementId.HasValue)
            {
                // Mappings spécifiques au département
                var raisons = await context.Set<DepartementRaisonSortie>()
                    .Where(m => m.DepartementId == departementId.Value)
                    .Include(m => m.RaisonSortie)
                        .ThenInclude(r => r!.Categorie)
                    .OrderBy(m => m.OrdreAffichage)
                    .Select(m => m.RaisonSortie)
                    .Where(r => r != null && r.EstActif)
                    .Cast<RaisonSortie>()
                    .ToListAsync();

                if (raisons.Count > 0)
                    return (IReadOnlyList<RaisonSortie>)raisons;
            }

            // Fallback : mappings par défaut (DepartementId = NULL)
            return (IReadOnlyList<RaisonSortie>)await context.Set<DepartementRaisonSortie>()
                .Where(m => m.DepartementId == null)
                .Include(m => m.RaisonSortie)
                    .ThenInclude(r => r!.Categorie)
                .OrderBy(m => m.OrdreAffichage)
                .Select(m => m.RaisonSortie)
                .Where(r => r != null && r.EstActif)
                .Cast<RaisonSortie>()
                .ToListAsync();
        }) ?? [];
    }

    /// <inheritdoc />
    public async Task<string?> GetDepartementCodeForRaisonAsync(string raisonSortieCode)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var mapping = await context.Set<DepartementRaisonSortie>()
            .Include(m => m.Departement)
            .Include(m => m.RaisonSortie)
            .Where(m => m.DepartementId != null
                     && m.RaisonSortie.Code == raisonSortieCode)
            .FirstOrDefaultAsync();

        return mapping?.Departement?.CodeDepartement;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DepartementRaisonSortie>> GetDepartementRaisonMappingsAsync(int? departementId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Set<DepartementRaisonSortie>()
            .Where(m => m.DepartementId == departementId)
            .Include(m => m.RaisonSortie)
            .OrderBy(m => m.OrdreAffichage)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task SaveDepartementRaisonMappingsAsync(int? departementId, List<int> raisonSortieIds)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        // Supprimer les mappings existants
        var existing = await context.Set<DepartementRaisonSortie>()
            .Where(m => m.DepartementId == departementId)
            .ToListAsync();
        context.Set<DepartementRaisonSortie>().RemoveRange(existing);

        // Recréer
        var newMappings = raisonSortieIds.Select((id, index) => new DepartementRaisonSortie
        {
            DepartementId = departementId,
            RaisonSortieId = id,
            AutoSelection = raisonSortieIds.Count == 1,
            OrdreAffichage = index + 1
        });
        await context.Set<DepartementRaisonSortie>().AddRangeAsync(newMappings);
        await context.SaveChangesAsync();

        // Invalider le cache
        _cache.Remove(CacheCategoriesKey);
        _cache.Remove(CacheCategoriesWithRaisonsKey);
    }
}
