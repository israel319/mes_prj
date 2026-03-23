using KCCMaterialFlow.Module.Shared.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des départements avec mise en cache de 1 heure.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class DepartementService : IDepartementService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DepartementService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private const string CacheKeyPrefix = "Departement_";
    private const string CacheKeyAll = "Departements_All";

    public DepartementService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<DepartementService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Departement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAll, out IReadOnlyList<Departement>? cachedDepts) && cachedDepts != null)
        {
            _logger.LogDebug("Départements récupérés depuis le cache ({Count})", cachedDepts.Count);
            return cachedDepts;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var departements = await context.Set<Departement>()
            .AsNoTracking()
            .Where(d => d.EstActif)
            .OrderBy(d => d.NomDepartement)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAll, (IReadOnlyList<Departement>)departements, CacheDuration);
        _logger.LogDebug("Départements mis en cache ({Count}) pour {Duration} heure(s)", departements.Count, CacheDuration.TotalHours);

        return departements;
    }

    /// <inheritdoc />
    public async Task<Departement?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}Id_{id}";

        if (_cache.TryGetValue(cacheKey, out Departement? cachedDept))
        {
            return cachedDept;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var departement = await context.Set<Departement>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.IdDepartement == id, cancellationToken);

        if (departement != null)
        {
            _cache.Set(cacheKey, departement, CacheDuration);
        }

        return departement;
    }

    /// <inheritdoc />
    public async Task<Departement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var cacheKey = $"{CacheKeyPrefix}Code_{code.ToUpperInvariant()}";

        if (_cache.TryGetValue(cacheKey, out Departement? cachedDept))
        {
            return cachedDept;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var departement = await context.Set<Departement>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.CodeDepartement.ToUpper() == code.ToUpper() && d.EstActif, cancellationToken);

        if (departement != null)
        {
            _cache.Set(cacheKey, departement, CacheDuration);
        }

        return departement;
    }

    /// <inheritdoc />
    public async Task<Departement> CreateAsync(Departement departement, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        departement.DateCreation = DateTime.Now;
        context.Set<Departement>().Add(departement);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Département {Code} créé", departement.CodeDepartement);

        return departement;
    }

    /// <inheritdoc />
    public async Task<Departement> UpdateAsync(Departement departement, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.Set<Departement>()
            .FirstOrDefaultAsync(d => d.IdDepartement == departement.IdDepartement, cancellationToken)
            ?? throw new InvalidOperationException($"Département {departement.IdDepartement} non trouvé");

        existing.CodeDepartement = departement.CodeDepartement;
        existing.NomDepartement = departement.NomDepartement;
        existing.Description = departement.Description;
        existing.ResponsableLogin = departement.ResponsableLogin;
        existing.ResponsableNom = departement.ResponsableNom;
        existing.ResponsableEmail = departement.ResponsableEmail;
        existing.EstActif = departement.EstActif;
        existing.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Département {Code} mis à jour", departement.CodeDepartement);

        return existing;
    }

    /// <inheritdoc />
    public void InvalidateCache()
    {
        _cache.Remove(CacheKeyAll);
        _logger.LogDebug("Cache des départements invalidé");
    }
}
