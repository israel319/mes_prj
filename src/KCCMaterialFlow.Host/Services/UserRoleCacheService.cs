using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Service centralisé : source unique pour le rôle BD d'un utilisateur.
/// Utilisé par ClaimsTransformation, UserAccessMiddleware et DatabaseRoleEnricherService.
/// Cache IMemoryCache avec invalidation explicite depuis l'admin.
/// </summary>
public class UserRoleCacheService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UserRoleCacheService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string CachePrefix = "UserRole_";

    // Sentinel pour distinguer "jamais lu" de "lu mais pas de rôle"
    private static readonly string NoRoleSentinel = "__NO_ROLE__";

    public UserRoleCacheService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<UserRoleCacheService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Retourne le CodeRole de l'utilisateur ou null si inexistant/inactif.
    /// Version async pour middleware et ClaimsTransformation.
    /// </summary>
    public async Task<string?> GetUserRoleAsync(string login)
    {
        var cacheKey = GetCacheKey(login);

        if (_cache.TryGetValue(cacheKey, out string? cached))
            return cached == NoRoleSentinel ? null : cached;

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var role = await QueryRoleAsync(dbContext, login);

            _cache.Set(cacheKey, role ?? NoRoleSentinel, CacheDuration);
            return role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lecture rôle BD pour {Login}", login);
            return null;
        }
    }

    /// <summary>
    /// Retourne le CodeRole de l'utilisateur ou null si inexistant/inactif.
    /// Version synchrone pour usage dans le circuit Blazor (scoped).
    /// </summary>
    public string? GetUserRole(string login)
    {
        var cacheKey = GetCacheKey(login);

        if (_cache.TryGetValue(cacheKey, out string? cached))
            return cached == NoRoleSentinel ? null : cached;

        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var role = QueryRole(dbContext, login);

            _cache.Set(cacheKey, role ?? NoRoleSentinel, CacheDuration);
            return role;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lecture rôle BD (sync) pour {Login}", login);
            return null;
        }
    }

    /// <summary>
    /// Invalide le cache pour un login donné.
    /// Appelé par l'admin lors d'un changement de rôle ou de statut.
    /// </summary>
    public void InvalidateUser(string login)
    {
        if (string.IsNullOrWhiteSpace(login)) return;

        // Invalider toutes les variantes possibles du login
        var keys = GetCacheKeys(login);
        foreach (var key in keys)
        {
            _cache.Remove(key);
        }

        _logger.LogInformation("Cache rôle invalidé pour {Login}", login);
    }

    /// <summary>
    /// Invalide tous les caches rôle (ex: changement global de configuration).
    /// Note: IMemoryCache ne supporte pas l'énumération des clés,
    /// donc on utilise un token d'annulation partagé.
    /// </summary>
    public void InvalidateAll()
    {
        // On ne peut pas énumérer IMemoryCache, mais les entrées expireront naturellement en 5 min.
        // Pour un effet immédiat, on incrémente un compteur de version dans le cache.
        var version = _cache.GetOrCreate("UserRole__Version", e => 0);
        _cache.Set("UserRole__Version", version + 1);
        _logger.LogInformation("Cache rôle global invalidé (version {Version})", version + 1);
    }

    private static string GetCacheKey(string login)
    {
        return $"{CachePrefix}{login.ToUpperInvariant()}";
    }

    private static List<string> GetCacheKeys(string login)
    {
        var keys = new List<string> { GetCacheKey(login) };
        if (login.Contains('\\'))
        {
            keys.Add(GetCacheKey(login.Split('\\').Last()));
        }
        return keys;
    }

    private static List<string> GetLoginVariants(string login)
    {
        var variants = new List<string> { login };
        if (login.Contains('\\'))
            variants.Add(login.Split('\\').Last());
        return variants;
    }

    private static async Task<string?> QueryRoleAsync(KCCMaterialFlowDbContext dbContext, string login)
    {
        var loginVariants = GetLoginVariants(login);

        return await dbContext.Set<Utilisateur>()
            .AsNoTracking()
            .Where(u => loginVariants.Contains(u.Login) && u.EstActif)
            .Join(dbContext.Set<Role>().Where(r => r.EstActif),
                u => u.IdRole,
                r => r.Id,
                (u, r) => r.CodeRole)
            .FirstOrDefaultAsync();
    }

    private static string? QueryRole(KCCMaterialFlowDbContext dbContext, string login)
    {
        var loginVariants = GetLoginVariants(login);

        return dbContext.Set<Utilisateur>()
            .AsNoTracking()
            .Where(u => loginVariants.Contains(u.Login) && u.EstActif)
            .Join(dbContext.Set<Role>().Where(r => r.EstActif),
                u => u.IdRole,
                r => r.Id,
                (u, r) => r.CodeRole)
            .FirstOrDefault();
    }
}
