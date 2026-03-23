using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using KCCMaterialFlow.Module.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des activités utilisateur avec mise en cache.
/// Permet d'assigner, retirer et vérifier les activités autorisées pour chaque utilisateur.
/// </summary>
public class ActiviteService : IActiviteService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ActiviteService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string CacheKeyAllActivites = "Activites_All";
    private const string CacheKeyUserPrefix = "Activites_User_";

    public ActiviteService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<ActiviteService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    // ===== ACTIVITÉS =====

    public async Task<IReadOnlyList<Activite>> GetAllActivitesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAllActivites, out IReadOnlyList<Activite>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var activites = await context.Set<Activite>()
            .AsNoTracking()
            .Where(a => a.EstActif)
            .OrderBy(a => a.Module)
            .ThenBy(a => a.OrdreAffichage)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAllActivites, (IReadOnlyList<Activite>)activites, CacheDuration);
        _logger.LogDebug("Cache activités chargé : {Count} activités", activites.Count);

        return activites;
    }

    public async Task<Activite?> GetActiviteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var all = await GetAllActivitesAsync(cancellationToken);
        return all.FirstOrDefault(a => a.IdActivite == id);
    }

    public async Task<Activite?> GetActiviteByCodeAsync(string codeActivite, CancellationToken cancellationToken = default)
    {
        var all = await GetAllActivitesAsync(cancellationToken);
        return all.FirstOrDefault(a => a.CodeActivite.Equals(codeActivite, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Dictionary<string, List<Activite>>> GetActivitesGroupedByModuleAsync(CancellationToken cancellationToken = default)
    {
        var all = await GetAllActivitesAsync(cancellationToken);
        return all
            .GroupBy(a => a.Module)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.OrderBy(a => a.OrdreAffichage).ToList());
    }

    // ===== ACTIVITÉS UTILISATEUR =====

    public async Task<IReadOnlyList<Activite>> GetActivitesForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyUserPrefix}{idUtilisateur}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Activite>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var activites = await context.Set<UtilisateurActivite>()
            .AsNoTracking()
            .Where(ua => ua.IdUtilisateur == idUtilisateur && ua.EstActif)
            .Include(ua => ua.Activite)
            .Where(ua => ua.Activite != null && ua.Activite.EstActif)
            .Select(ua => ua.Activite!)
            .OrderBy(a => a.Module)
            .ThenBy(a => a.OrdreAffichage)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Activite>)activites, CacheDuration);
        _logger.LogDebug("Cache activités utilisateur {IdUtilisateur} chargé : {Count} activités", idUtilisateur, activites.Count);

        return activites;
    }

    public async Task<IReadOnlyList<int>> GetActiviteIdsForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default)
    {
        var activites = await GetActivitesForUserAsync(idUtilisateur, cancellationToken);
        return activites.Select(a => a.IdActivite).ToList();
    }

    public async Task UpdateUserActivitesAsync(int idUtilisateur, IEnumerable<int> activiteIds, string? attribueParLogin = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var newIds = activiteIds.ToHashSet();

        // Récupérer les assignations existantes
        var existing = await context.Set<UtilisateurActivite>()
            .Where(ua => ua.IdUtilisateur == idUtilisateur)
            .ToListAsync(cancellationToken);

        var existingIds = existing.Select(ua => ua.IdActivite).ToHashSet();

        // Supprimer celles qui ne sont plus sélectionnées
        var toRemove = existing.Where(ua => !newIds.Contains(ua.IdActivite)).ToList();
        if (toRemove.Count > 0)
        {
            context.Set<UtilisateurActivite>().RemoveRange(toRemove);
            _logger.LogInformation("Retrait de {Count} activités pour l'utilisateur {IdUtilisateur}", toRemove.Count, idUtilisateur);
        }

        // Ajouter les nouvelles
        var toAdd = newIds.Except(existingIds).ToList();
        if (toAdd.Count > 0)
        {
            foreach (var activiteId in toAdd)
            {
                context.Set<UtilisateurActivite>().Add(new UtilisateurActivite
                {
                    IdUtilisateur = idUtilisateur,
                    IdActivite = activiteId,
                    DateAttribution = DateTime.Now,
                    AttribueParLogin = attribueParLogin,
                    EstActif = true
                });
            }
            _logger.LogInformation("Ajout de {Count} activités pour l'utilisateur {IdUtilisateur}", toAdd.Count, idUtilisateur);
        }

        await context.SaveChangesAsync(cancellationToken);
        InvalidateUserCache(idUtilisateur);
    }

    public async Task<bool> AssignActiviteToUserAsync(int idUtilisateur, int idActivite, string? attribueParLogin = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Vérifier si l'assignation existe déjà
        var exists = await context.Set<UtilisateurActivite>()
            .AnyAsync(ua => ua.IdUtilisateur == idUtilisateur && ua.IdActivite == idActivite, cancellationToken);

        if (exists)
        {
            _logger.LogDebug("Activité {IdActivite} déjà assignée à l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
            return false;
        }

        context.Set<UtilisateurActivite>().Add(new UtilisateurActivite
        {
            IdUtilisateur = idUtilisateur,
            IdActivite = idActivite,
            DateAttribution = DateTime.Now,
            AttribueParLogin = attribueParLogin,
            EstActif = true
        });

        await context.SaveChangesAsync(cancellationToken);
        InvalidateUserCache(idUtilisateur);

        _logger.LogInformation("Activité {IdActivite} assignée à l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
        return true;
    }

    public async Task<bool> RemoveActiviteFromUserAsync(int idUtilisateur, int idActivite, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entry = await context.Set<UtilisateurActivite>()
            .FirstOrDefaultAsync(ua => ua.IdUtilisateur == idUtilisateur && ua.IdActivite == idActivite, cancellationToken);

        if (entry == null)
        {
            _logger.LogDebug("Activité {IdActivite} non trouvée pour l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
            return false;
        }

        context.Set<UtilisateurActivite>().Remove(entry);
        await context.SaveChangesAsync(cancellationToken);
        InvalidateUserCache(idUtilisateur);

        _logger.LogInformation("Activité {IdActivite} retirée de l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
        return true;
    }

    public async Task<bool> UserHasActiviteAsync(int idUtilisateur, string codeActivite, CancellationToken cancellationToken = default)
    {
        var activites = await GetActivitesForUserAsync(idUtilisateur, cancellationToken);
        return activites.Any(a => a.CodeActivite.Equals(codeActivite, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> UserHasAnyActiviteAsync(int idUtilisateur, IEnumerable<string> codeActivites, CancellationToken cancellationToken = default)
    {
        var activites = await GetActivitesForUserAsync(idUtilisateur, cancellationToken);
        var codes = new HashSet<string>(codeActivites, StringComparer.OrdinalIgnoreCase);
        return activites.Any(a => codes.Contains(a.CodeActivite));
    }

    public void InvalidateUserCache(int idUtilisateur)
    {
        var cacheKey = $"{CacheKeyUserPrefix}{idUtilisateur}";
        _cache.Remove(cacheKey);
        _logger.LogDebug("Cache activités invalidé pour l'utilisateur {IdUtilisateur}", idUtilisateur);
    }
}
