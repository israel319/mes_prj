using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des barrières avec mise en cache de 1 heure.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class BarriereService : IBarriereService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<BarriereService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private const string CacheKeyPrefix = "Barriere_";
    private const string CacheKeyAll = "Barrieres_All";

    public BarriereService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<BarriereService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Barriere>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAll, out IReadOnlyList<Barriere>? cachedBarrieres) && cachedBarrieres != null)
        {
            _logger.LogDebug("Barrières récupérées depuis le cache ({Count})", cachedBarrieres.Count);
            return cachedBarrieres;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var barrieres = await context.Set<Barriere>()
            .AsNoTracking()
            .Where(b => b.EstActive)
            .OrderBy(b => b.NomBarriere)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAll, (IReadOnlyList<Barriere>)barrieres, CacheDuration);
        _logger.LogDebug("Barrières mises en cache ({Count}) pour {Duration} heure(s)", barrieres.Count, CacheDuration.TotalHours);

        return barrieres;
    }

    /// <inheritdoc />
    public async Task<Barriere?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}Id_{id}";

        if (_cache.TryGetValue(cacheKey, out Barriere? cachedBarriere))
        {
            return cachedBarriere;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var barriere = await context.Set<Barriere>()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (barriere != null)
        {
            _cache.Set(cacheKey, barriere, CacheDuration);
        }

        return barriere;
    }

    /// <inheritdoc />
    public async Task<Barriere?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var cacheKey = $"{CacheKeyPrefix}Code_{code.ToUpperInvariant()}";

        if (_cache.TryGetValue(cacheKey, out Barriere? cachedBarriere))
        {
            return cachedBarriere;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var barriere = await context.Set<Barriere>()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.CodeBarriere.ToUpper() == code.ToUpper() && b.EstActive, cancellationToken);

        if (barriere != null)
        {
            _cache.Set(cacheKey, barriere, CacheDuration);
        }

        return barriere;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Barriere>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(type))
            return [];

        var cacheKey = $"{CacheKeyPrefix}Type_{type.ToUpperInvariant()}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Barriere>? cachedBarrieres) && cachedBarrieres != null)
        {
            return cachedBarrieres;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var barrieres = await context.Set<Barriere>()
            .AsNoTracking()
            .Where(b => b.TypeBarriere.ToUpper() == type.ToUpper() && b.EstActive)
            .OrderBy(b => b.NomBarriere)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Barriere>)barrieres, CacheDuration);

        return barrieres;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Barriere>> SearchByLocalisationAsync(string localisation, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(localisation))
            return [];

        var term = localisation.ToUpper();

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var barrieres = await context.Set<Barriere>()
            .AsNoTracking()
            .Where(b => b.EstActive && b.Localisation.ToUpper().Contains(term))
            .OrderBy(b => b.NomBarriere)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Recherche barrières par localisation '{Term}' : {Count} résultats", localisation, barrieres.Count);

        return barrieres;
    }

    /// <inheritdoc />
    public async Task<Barriere> CreateAsync(Barriere barriere, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        barriere.DateCreation = DateTime.Now;

        context.Set<Barriere>().Add(barriere);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Barrière {Code} créée", barriere.CodeBarriere);

        return barriere;
    }

    /// <inheritdoc />
    public async Task<Barriere> UpdateAsync(Barriere barriere, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.Set<Barriere>()
            .FirstOrDefaultAsync(b => b.Id == barriere.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Barrière {barriere.Id} non trouvée");

        existing.CodeBarriere = barriere.CodeBarriere;
        existing.NomBarriere = barriere.NomBarriere;
        existing.Localisation = barriere.Localisation;
        existing.Description = barriere.Description;
        existing.TypeBarriere = barriere.TypeBarriere;
        existing.EstActive = barriere.EstActive;
        existing.HorairesOuverture = barriere.HorairesOuverture;
        existing.Telephone = barriere.Telephone;
        existing.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Barrière {Code} mise à jour", barriere.CodeBarriere);

        return existing;
    }

    /// <inheritdoc />
    public void InvalidateCache()
    {
        _cache.Remove(CacheKeyAll);
        _logger.LogDebug("Cache des barrières invalidé");
    }
}
