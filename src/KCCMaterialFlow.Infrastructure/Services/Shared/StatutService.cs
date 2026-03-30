using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des statuts avec mise en cache.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class StatutService : IStatutService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<StatutService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(60);
    private const string CacheKeyAll = "Statuts_All";
    private const string CacheKeyPrefix = "Statut_";

    public StatutService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<StatutService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Statut>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAll, out IReadOnlyList<Statut>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var statuts = await context.Set<Statut>()
            .AsNoTracking()
            .Where(s => s.EstActif)
            .OrderBy(s => s.Ordre)
            .ThenBy(s => s.LibelleStatut)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAll, (IReadOnlyList<Statut>)statuts, CacheDuration);
        return statuts;
    }

    public async Task<Statut?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        
        if (_cache.TryGetValue(cacheKey, out Statut? cached))
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var statut = await context.Set<Statut>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (statut != null)
            _cache.Set(cacheKey, statut, CacheDuration);

        return statut;
    }

    public async Task<Statut?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Statut>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.CodeStatut.ToUpper() == code.ToUpper() && s.EstActif, cancellationToken);
    }

    public async Task<IReadOnlyList<Statut>> GetByTypeBonAsync(string typeBon, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}Type_{typeBon}";
        
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Statut>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var statuts = await context.Set<Statut>()
            .AsNoTracking()
            .Where(s => s.EstActif && (s.TypeBon == "Tous" || s.TypeBon == typeBon))
            .OrderBy(s => s.Ordre)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Statut>)statuts, CacheDuration);
        return statuts;
    }

    public async Task<Statut> CreateAsync(Statut statut, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        statut.DateCreation = DateTime.Now;
        statut.CodeStatut = statut.CodeStatut.ToUpperInvariant();

        context.Set<Statut>().Add(statut);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Statut créé: {Code} - {Libelle}", statut.CodeStatut, statut.LibelleStatut);

        return statut;
    }

    public async Task<Statut> UpdateAsync(Statut statut, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.Set<Statut>()
            .FirstOrDefaultAsync(s => s.Id == statut.Id, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Statut {statut.Id} non trouvé");

        if (existing.EstSysteme)
        {
            existing.LibelleStatut = statut.LibelleStatut;
            existing.Description = statut.Description;
            existing.CouleurFond = statut.CouleurFond;
            existing.CouleurTexte = statut.CouleurTexte;
            existing.Icone = statut.Icone;
        }
        else
        {
            existing.CodeStatut = statut.CodeStatut.ToUpperInvariant();
            existing.LibelleStatut = statut.LibelleStatut;
            existing.Description = statut.Description;
            existing.TypeBon = statut.TypeBon;
            existing.CouleurFond = statut.CouleurFond;
            existing.CouleurTexte = statut.CouleurTexte;
            existing.Icone = statut.Icone;
            existing.Ordre = statut.Ordre;
            existing.EstFinal = statut.EstFinal;
            existing.RequiertAction = statut.RequiertAction;
            existing.StatutsSuivants = statut.StatutsSuivants;
            existing.EstActif = statut.EstActif;
        }

        existing.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Statut modifié: {Code}", existing.CodeStatut);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var statut = await context.Set<Statut>()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (statut == null)
            return false;

        if (statut.EstSysteme)
            throw new InvalidOperationException("Les statuts système ne peuvent pas être supprimés");

        statut.EstActif = false;
        statut.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Statut supprimé: {Code}", statut.CodeStatut);
        return true;
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Set<Statut>()
            .Where(s => s.CodeStatut.ToUpper() == code.ToUpper());

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Statut>> GetNextStatusesAsync(int statutId, CancellationToken cancellationToken = default)
    {
        var statut = await GetByIdAsync(statutId, cancellationToken);
        if (statut == null || string.IsNullOrWhiteSpace(statut.StatutsSuivants))
            return [];

        var nextIds = statut.StatutsSuivants
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var id) ? id : 0)
            .Where(id => id > 0)
            .ToList();

        if (nextIds.Count == 0)
            return [];

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Statut>()
            .AsNoTracking()
            .Where(s => nextIds.Contains(s.Id) && s.EstActif)
            .OrderBy(s => s.Ordre)
            .ToListAsync(cancellationToken);
    }

    private void InvalidateCache()
    {
        _cache.Remove(CacheKeyAll);
    }
}
