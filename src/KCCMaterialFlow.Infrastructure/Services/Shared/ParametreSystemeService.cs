using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using KCCMaterialFlow.Module.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des paramètres système avec mise en cache.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class ParametreSystemeService : IParametreSystemeService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ParametreSystemeService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(60);
    private const string CacheKeyAll = "ParametresSysteme_All";
    private const string CacheKeyPrefix = "ParametreSysteme_";
    private const string CacheKeyCategories = "ParametresSysteme_Categories";

    public ParametreSystemeService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<ParametreSystemeService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ParametreSysteme>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAll, out IReadOnlyList<ParametreSysteme>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var parametres = await context.Set<ParametreSysteme>()
            .AsNoTracking()
            .Where(p => p.EstVisible)
            .OrderBy(p => p.Categorie)
            .ThenBy(p => p.Ordre)
            .ThenBy(p => p.Libelle)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAll, (IReadOnlyList<ParametreSysteme>)parametres, CacheDuration);
        return parametres;
    }

    public async Task<ParametreSysteme?> GetByKeyAsync(string cle, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cle))
            return null;

        var cacheKey = $"{CacheKeyPrefix}{cle.ToUpperInvariant()}";
        
        if (_cache.TryGetValue(cacheKey, out ParametreSysteme? cached))
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var parametre = await context.Set<ParametreSysteme>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Cle.ToUpper() == cle.ToUpper(), cancellationToken);

        if (parametre != null)
            _cache.Set(cacheKey, parametre, CacheDuration);

        return parametre;
    }

    public async Task<string?> GetValueAsync(string cle, CancellationToken cancellationToken = default)
    {
        var parametre = await GetByKeyAsync(cle, cancellationToken);
        return parametre?.Valeur;
    }

    public async Task<int> GetIntValueAsync(string cle, int defaultValue = 0, CancellationToken cancellationToken = default)
    {
        var valeur = await GetValueAsync(cle, cancellationToken);
        return int.TryParse(valeur, out var result) ? result : defaultValue;
    }

    public async Task<bool> GetBoolValueAsync(string cle, bool defaultValue = false, CancellationToken cancellationToken = default)
    {
        var valeur = await GetValueAsync(cle, cancellationToken);
        return bool.TryParse(valeur, out var result) ? result : defaultValue;
    }

    public async Task<IReadOnlyList<ParametreSysteme>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categorie))
            return [];

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<ParametreSysteme>()
            .AsNoTracking()
            .Where(p => p.EstVisible && p.Categorie.ToUpper() == categorie.ToUpper())
            .OrderBy(p => p.Ordre)
            .ThenBy(p => p.Libelle)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateValueAsync(string cle, string valeur, string modifieParLogin, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var parametre = await context.Set<ParametreSysteme>()
            .FirstOrDefaultAsync(p => p.Cle.ToUpper() == cle.ToUpper(), cancellationToken);

        if (parametre == null)
            return false;

        if (!parametre.EstModifiable)
            throw new InvalidOperationException($"Le paramètre {cle} n'est pas modifiable");

        var ancienneValeur = parametre.Valeur;
        parametre.Valeur = valeur;
        parametre.ModifieParLogin = modifieParLogin;
        parametre.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Paramètre {Cle} modifié de '{Ancien}' à '{Nouveau}' par {User}",
            cle, ancienneValeur, valeur, modifieParLogin);

        return true;
    }

    public async Task<ParametreSysteme> CreateAsync(ParametreSysteme parametre, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        parametre.Cle = parametre.Cle.ToUpperInvariant();

        context.Set<ParametreSysteme>().Add(parametre);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Paramètre créé: {Cle}", parametre.Cle);

        return parametre;
    }

    public async Task<ParametreSysteme> UpdateAsync(ParametreSysteme parametre, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.Set<ParametreSysteme>()
            .FirstOrDefaultAsync(p => p.IdParametre == parametre.IdParametre, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Paramètre {parametre.IdParametre} non trouvé");

        if (existing.EstSysteme)
        {
            existing.Valeur = parametre.Valeur;
        }
        else
        {
            existing.Cle = parametre.Cle.ToUpperInvariant();
            existing.Valeur = parametre.Valeur;
            existing.TypeDonnee = parametre.TypeDonnee;
            existing.Categorie = parametre.Categorie;
            existing.Libelle = parametre.Libelle;
            existing.Description = parametre.Description;
            existing.ValeurDefaut = parametre.ValeurDefaut;
            existing.ValeursPossibles = parametre.ValeursPossibles;
            existing.ValeurMin = parametre.ValeurMin;
            existing.ValeurMax = parametre.ValeurMax;
            existing.Unite = parametre.Unite;
            existing.Ordre = parametre.Ordre;
            existing.NecessiteRedemarrage = parametre.NecessiteRedemarrage;
            existing.EstVisible = parametre.EstVisible;
            existing.EstModifiable = parametre.EstModifiable;
        }

        existing.ModifieParLogin = parametre.ModifieParLogin;
        existing.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Paramètre modifié: {Cle}", existing.Cle);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var parametre = await context.Set<ParametreSysteme>()
            .FirstOrDefaultAsync(p => p.IdParametre == id, cancellationToken);

        if (parametre == null)
            return false;

        if (parametre.EstSysteme)
            throw new InvalidOperationException("Les paramètres système ne peuvent pas être supprimés");

        context.Set<ParametreSysteme>().Remove(parametre);
        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Paramètre supprimé: {Cle}", parametre.Cle);
        return true;
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyCategories, out IReadOnlyList<string>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var categories = await context.Set<ParametreSysteme>()
            .AsNoTracking()
            .Where(p => p.EstVisible)
            .Select(p => p.Categorie)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyCategories, (IReadOnlyList<string>)categories, CacheDuration);
        return categories;
    }

    public void InvalidateCache()
    {
        _cache.Remove(CacheKeyAll);
        _cache.Remove(CacheKeyCategories);
    }
}
