using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using KCCMaterialFlow.Module.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des types de matériel avec mise en cache.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class TypeMaterielService : ITypeMaterielService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TypeMaterielService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(60);
    private const string CacheKeyAll = "TypesMateriels_All";
    private const string CacheKeyPrefix = "TypeMateriel_";
    private const string CacheKeyCategories = "TypesMateriels_Categories";

    public TypeMaterielService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<TypeMaterielService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TypeMateriel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAll, out IReadOnlyList<TypeMateriel>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var types = await context.Set<TypeMateriel>()
            .AsNoTracking()
            .Where(t => t.EstActif)
            .OrderBy(t => t.Ordre)
            .ThenBy(t => t.NomType)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAll, (IReadOnlyList<TypeMateriel>)types, CacheDuration);
        return types;
    }

    public async Task<TypeMateriel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        
        if (_cache.TryGetValue(cacheKey, out TypeMateriel? cached))
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var type = await context.Set<TypeMateriel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdTypeMateriel == id, cancellationToken);

        if (type != null)
            _cache.Set(cacheKey, type, CacheDuration);

        return type;
    }

    public async Task<TypeMateriel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<TypeMateriel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.CodeType.ToUpper() == code.ToUpper() && t.EstActif, cancellationToken);
    }

    public async Task<IReadOnlyList<TypeMateriel>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categorie))
            return [];

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<TypeMateriel>()
            .AsNoTracking()
            .Where(t => t.EstActif && t.Categorie != null && t.Categorie.ToUpper() == categorie.ToUpper())
            .OrderBy(t => t.Ordre)
            .ThenBy(t => t.NomType)
            .ToListAsync(cancellationToken);
    }

    public async Task<TypeMateriel> CreateAsync(TypeMateriel typeMateriel, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        typeMateriel.DateCreation = DateTime.Now;
        typeMateriel.CodeType = typeMateriel.CodeType.ToUpperInvariant();

        context.Set<TypeMateriel>().Add(typeMateriel);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Type de matériel créé: {Code} - {Nom}", typeMateriel.CodeType, typeMateriel.NomType);

        return typeMateriel;
    }

    public async Task<TypeMateriel> UpdateAsync(TypeMateriel typeMateriel, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.Set<TypeMateriel>()
            .FirstOrDefaultAsync(t => t.IdTypeMateriel == typeMateriel.IdTypeMateriel, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Type de matériel {typeMateriel.IdTypeMateriel} non trouvé");

        existing.CodeType = typeMateriel.CodeType.ToUpperInvariant();
        existing.NomType = typeMateriel.NomType;
        existing.Description = typeMateriel.Description;
        existing.Categorie = typeMateriel.Categorie;
        existing.Icone = typeMateriel.Icone;
        existing.Couleur = typeMateriel.Couleur;
        existing.RequiertApprobationDepartement = typeMateriel.RequiertApprobationDepartement;
        existing.RequiertApprobationDirection = typeMateriel.RequiertApprobationDirection;
        existing.NiveauxApprobation = typeMateriel.NiveauxApprobation;
        existing.DureeValiditeDefautJours = typeMateriel.DureeValiditeDefautJours;
        existing.DureeMaximumJours = typeMateriel.DureeMaximumJours;
        existing.NumeroSerieObligatoire = typeMateriel.NumeroSerieObligatoire;
        existing.PhotoObligatoire = typeMateriel.PhotoObligatoire;
        existing.ChampsPersonnalises = typeMateriel.ChampsPersonnalises;
        existing.WorkflowConfig = typeMateriel.WorkflowConfig;
        existing.Ordre = typeMateriel.Ordre;
        existing.EstActif = typeMateriel.EstActif;
        existing.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Type de matériel modifié: {Code}", existing.CodeType);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var type = await context.Set<TypeMateriel>()
            .FirstOrDefaultAsync(t => t.IdTypeMateriel == id, cancellationToken);

        if (type == null)
            return false;

        type.EstActif = false;
        type.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Type de matériel supprimé: {Code}", type.CodeType);
        return true;
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Set<TypeMateriel>()
            .Where(t => t.CodeType.ToUpper() == code.ToUpper());

        if (excludeId.HasValue)
            query = query.Where(t => t.IdTypeMateriel != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyCategories, out IReadOnlyList<string>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var categories = await context.Set<TypeMateriel>()
            .AsNoTracking()
            .Where(t => t.EstActif && t.Categorie != null)
            .Select(t => t.Categorie!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyCategories, (IReadOnlyList<string>)categories, CacheDuration);
        return categories;
    }

    private void InvalidateCache()
    {
        _cache.Remove(CacheKeyAll);
        _cache.Remove(CacheKeyCategories);
    }
}
