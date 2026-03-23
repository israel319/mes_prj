using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using KCCMaterialFlow.Module.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des utilisateurs avec mise en cache de 30 minutes.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class UtilisateurService : IUtilisateurService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UtilisateurService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string CacheKeyPrefix = "Utilisateur_";
    private const string CacheKeyAllActive = "Utilisateurs_AllActive";

    public UtilisateurService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<UtilisateurService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Utilisateur?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(login))
            return null;

        var cacheKey = $"{CacheKeyPrefix}Login_{login.ToUpperInvariant()}";

        if (_cache.TryGetValue(cacheKey, out Utilisateur? cachedUser))
        {
            _logger.LogDebug("Utilisateur {Login} récupéré depuis le cache", login);
            return cachedUser;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Set<Utilisateur>()
            .AsNoTracking()
            .Include(u => u.RolePrincipal)
            .FirstOrDefaultAsync(u => u.Login.ToUpper() == login.ToUpper() && u.EstActif, cancellationToken);

        if (user != null)
        {
            _cache.Set(cacheKey, user, CacheDuration);
            _logger.LogDebug("Utilisateur {Login} mis en cache pour {Duration} minutes", login, CacheDuration.TotalMinutes);
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Utilisateur>> GetByDepartementAsync(string departement, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(departement))
            return [];

        var cacheKey = $"{CacheKeyPrefix}Dept_{departement.ToUpperInvariant()}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Utilisateur>? cachedUsers) && cachedUsers != null)
        {
            _logger.LogDebug("Utilisateurs du département {Departement} récupérés depuis le cache ({Count})", departement, cachedUsers.Count);
            return cachedUsers;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var users = await context.Set<Utilisateur>()
            .AsNoTracking()
            .Include(u => u.RolePrincipal)
            .Where(u => u.Departement != null && u.Departement.ToUpper() == departement.ToUpper() && u.EstActif)
            .OrderBy(u => u.NomComplet)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Utilisateur>)users, CacheDuration);
        _logger.LogDebug("Utilisateurs du département {Departement} mis en cache ({Count})", departement, users.Count);

        return users;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Utilisateur>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            return [];

        var term = searchTerm.ToUpper();

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var users = await context.Set<Utilisateur>()
            .AsNoTracking()
            .Include(u => u.RolePrincipal)
            .Where(u => u.EstActif &&
                (u.NomComplet.ToUpper().Contains(term) ||
                 u.Login.ToUpper().Contains(term) ||
                 (u.Fonction != null && u.Fonction.ToUpper().Contains(term))))
            .OrderBy(u => u.NomComplet)
            .Take(50)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Recherche utilisateurs '{Term}' : {Count} résultats", searchTerm, users.Count);

        return users;
    }

    /// <inheritdoc />
    public async Task<Utilisateur?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}Id_{id}";

        if (_cache.TryGetValue(cacheKey, out Utilisateur? cachedUser))
        {
            return cachedUser;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Set<Utilisateur>()
            .AsNoTracking()
            .Include(u => u.RolePrincipal)
            .FirstOrDefaultAsync(u => u.IdUtilisateur == id, cancellationToken);

        if (user != null)
        {
            _cache.Set(cacheKey, user, CacheDuration);
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Utilisateur>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAllActive, out IReadOnlyList<Utilisateur>? cachedUsers) && cachedUsers != null)
        {
            _logger.LogDebug("Tous les utilisateurs actifs récupérés depuis le cache ({Count})", cachedUsers.Count);
            return cachedUsers;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var users = await context.Set<Utilisateur>()
            .AsNoTracking()
            .Include(u => u.RolePrincipal)
            .Where(u => u.EstActif)
            .OrderBy(u => u.NomComplet)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAllActive, (IReadOnlyList<Utilisateur>)users, CacheDuration);
        _logger.LogDebug("Tous les utilisateurs actifs mis en cache ({Count})", users.Count);

        return users;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Utilisateur>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "Utilisateurs_All";
        
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Utilisateur>? cachedUsers) && cachedUsers != null)
        {
            _logger.LogDebug("Tous les utilisateurs récupérés depuis le cache ({Count})", cachedUsers.Count);
            return cachedUsers;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var users = await context.Set<Utilisateur>()
            .AsNoTracking()
            .Include(u => u.RolePrincipal)
            .OrderBy(u => u.NomComplet)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Utilisateur>)users, CacheDuration);
        _logger.LogDebug("Tous les utilisateurs mis en cache ({Count})", users.Count);

        return users;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Utilisateur>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(role))
            return [];

        var cacheKey = $"{CacheKeyPrefix}Role_{role.ToUpperInvariant()}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Utilisateur>? cachedUsers) && cachedUsers != null)
        {
            return cachedUsers;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var users = await context.Set<Utilisateur>()
            .AsNoTracking()
            .Include(u => u.RolePrincipal)
            .Where(u => u.RolePrincipal != null && u.RolePrincipal.CodeRole.ToUpper() == role.ToUpper() && u.EstActif)
            .OrderBy(u => u.NomComplet)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Utilisateur>)users, CacheDuration);

        return users;
    }

    /// <inheritdoc />
    public async Task<Utilisateur> UpsertAsync(Utilisateur utilisateur, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var existing = await context.Set<Utilisateur>()
            .FirstOrDefaultAsync(u => u.Login.ToUpper() == utilisateur.Login.ToUpper(), cancellationToken);

        if (existing != null)
        {
            existing.NomComplet = utilisateur.NomComplet;
            existing.Fonction = utilisateur.Fonction;
            existing.Departement = utilisateur.Departement;
            existing.Email = utilisateur.Email;
            existing.Telephone = utilisateur.Telephone;
            existing.IdRole = utilisateur.IdRole;
            existing.EstActif = utilisateur.EstActif;
            existing.DateModification = DateTime.Now;

            context.Set<Utilisateur>().Update(existing);
            await context.SaveChangesAsync(cancellationToken);

            InvalidateCache();
            _logger.LogInformation("Utilisateur {Login} mis à jour (IdRole: {IdRole}, Actif: {Actif})", 
                utilisateur.Login, utilisateur.IdRole, utilisateur.EstActif);

            return existing;
        }
        else
        {
            utilisateur.DateCreation = DateTime.Now;
            context.Set<Utilisateur>().Add(utilisateur);
            await context.SaveChangesAsync(cancellationToken);

            InvalidateCache();
            _logger.LogInformation("Utilisateur {Login} créé (IdRole: {IdRole})", 
                utilisateur.Login, utilisateur.IdRole);

            return utilisateur;
        }
    }

    /// <inheritdoc />
    public async Task UpdateLastLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var user = await context.Set<Utilisateur>()
            .FirstOrDefaultAsync(u => u.Login.ToUpper() == login.ToUpper(), cancellationToken);

        if (user != null)
        {
            user.DerniereConnexion = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            
            var cacheKey = $"{CacheKeyPrefix}Login_{login.ToUpperInvariant()}";
            _cache.Remove(cacheKey);
        }
    }

    /// <inheritdoc />
    public void InvalidateCache()
    {
        _cache.Remove(CacheKeyAllActive);
        _cache.Remove("Utilisateurs_All");
        _logger.LogDebug("Cache des utilisateurs invalidé");
    }
}
