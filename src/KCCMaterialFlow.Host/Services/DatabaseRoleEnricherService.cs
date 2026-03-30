using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Service qui enrichit les rôles utilisateur avec ceux de la base de données.
/// Combine les rôles AD (Windows Groups) avec les rôles DB (table UtilisateurRoles).
/// Vérifie également les activités assignées à l'utilisateur.
/// </summary>
public class DatabaseRoleEnricherService : ICurrentUserService
{
    private readonly ICurrentUserService _innerService;
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly DevRoleSwitcherService _devRoleSwitcher;
    private readonly ILogger<DatabaseRoleEnricherService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string CacheKeyPrefix = "DbRoles_";
    private const string ActiviteCacheKeyPrefix = "DbActivites_";

    public DatabaseRoleEnricherService(
        ICurrentUserService innerService,
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        DevRoleSwitcherService devRoleSwitcher,
        ILogger<DatabaseRoleEnricherService> logger)
    {
        _innerService = innerService;
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _devRoleSwitcher = devRoleSwitcher;
        _logger = logger;
    }

    public CurrentUserInfo? GetCurrentUser()
    {
        var user = _innerService.GetCurrentUser();
        if (user == null) return null;

        // Enrichir avec les rôles de la base de données
        var dbRoles = GetDatabaseRoles(user.Login);
        if (dbRoles.Any())
        {
            var allRoles = user.Roles.ToList();
            foreach (var role in dbRoles)
            {
                if (!allRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                {
                    allRoles.Add(role);
                }
            }
            user.Roles = allRoles;
        }

        return user;
    }

    public string GetUserLogin()
    {
        // En mode simulation avec un utilisateur spécifique, retourner le login simulé
        if (_devRoleSwitcher.IsSimulationActive && !string.IsNullOrEmpty(_devRoleSwitcher.SimulatedUserLogin))
        {
            return _devRoleSwitcher.SimulatedUserLogin;
        }
        return _innerService.GetUserLogin();
    }

    public string GetUserDisplayName()
    {
        // En mode simulation avec un utilisateur spécifique, retourner le nom simulé
        if (_devRoleSwitcher.IsSimulationActive && !string.IsNullOrEmpty(_devRoleSwitcher.SimulatedUserName))
        {
            return _devRoleSwitcher.SimulatedUserName;
        }
        return _innerService.GetUserDisplayName();
    }

    public string? GetUserEmail() => _innerService.GetUserEmail();
    public string? GetUserDepartment() => _innerService.GetUserDepartment();
    public bool IsAuthenticated() => _innerService.IsAuthenticated();

    /// <inheritdoc />
    public void SetSimulatedUser(string login, string displayName, string? email, string? department, IEnumerable<string> roles)
        => _innerService.SetSimulatedUser(login, displayName, email, department, roles);

    /// <inheritdoc />
    public void ClearSimulation() => _innerService.ClearSimulation();

    /// <inheritdoc />
    public bool IsSimulationActive => _innerService.IsSimulationActive;

    public bool IsInRole(string role)
    {
        var roles = GetUserRoles();
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsInAnyRole(params string[] roles)
    {
        var userRoles = GetUserRoles();
        return roles.Any(r => userRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }

    public IEnumerable<string> GetUserRoles()
    {
        // En mode simulation, retourner les rôles simulés
        if (_devRoleSwitcher.IsSimulationActive)
        {
            _logger.LogDebug("DevRoleSwitcher actif - Rôles simulés: {Roles}",
                string.Join(", ", _devRoleSwitcher.SimulatedRoles));
            return _devRoleSwitcher.SimulatedRoles;
        }
        // Sinon, retourner les vrais rôles enrichis
        var user = GetCurrentUser();
        return user?.Roles ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Récupère les rôles de l'utilisateur depuis la base de données
    /// </summary>
    private List<string> GetDatabaseRoles(string login)
    {
        var cacheKey = $"{CacheKeyPrefix}{login}";

        if (_cache.TryGetValue(cacheKey, out List<string>? cachedRoles) && cachedRoles != null)
        {
            return cachedRoles;
        }

        try
        {
            // Créer un nouveau DbContext pour cette opération (thread-safe)
            using var dbContext = _dbContextFactory.CreateDbContext();
            
            // Chercher l'utilisateur par son login (avec ou sans domaine)
            var loginVariants = new List<string> { login };
            if (login.Contains('\\'))
            {
                loginVariants.Add(login.Split('\\').Last()); // Sans domaine
            }

            var utilisateur = dbContext.Set<Utilisateur>()
                .AsNoTracking()
                .FirstOrDefault(u => loginVariants.Contains(u.Login) && u.EstActif);

            if (utilisateur == null)
            {
                _logger.LogDebug("Utilisateur {Login} non trouvé en base de données", login);
                return new List<string>();
            }

            // Récupérer les rôles via UtilisateurRoles
            var roles = dbContext.Set<UtilisateurRole>()
                .AsNoTracking()
                .Where(ur => ur.IdUtilisateur == utilisateur.Id)
                .Join(dbContext.Set<Role>().Where(r => r.EstActif),
                    ur => ur.IdRole,
                    r => r.Id,
                    (ur, r) => r.CodeRole)
                .ToList();

            // Ajouter le rôle principal via la FK IdRole
            var directRole = dbContext.Set<Role>()
                .AsNoTracking()
                .Where(r => r.Id == utilisateur.IdRole && r.EstActif)
                .Select(r => r.CodeRole)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(directRole) && 
                !roles.Contains(directRole, StringComparer.OrdinalIgnoreCase))
            {
                roles.Add(directRole);
            }

            _logger.LogDebug("Rôles DB pour {Login}: {Roles}", login, string.Join(", ", roles));

            _cache.Set(cacheKey, roles, CacheDuration);
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des rôles DB pour {Login}", login);
            return new List<string>();
        }
    }

    /// <inheritdoc />
    public bool HasActivite(string codeActivite)
    {
        var login = GetUserLogin();
        var activites = GetDatabaseActivites(login);
        return activites.Contains(codeActivite, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public bool HasAnyActivite(params string[] codeActivites)
    {
        var login = GetUserLogin();
        var activites = GetDatabaseActivites(login);
        return codeActivites.Any(code => activites.Contains(code, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Récupère les codes d'activités de l'utilisateur depuis la base de données (avec cache)
    /// </summary>
    private HashSet<string> GetDatabaseActivites(string login)
    {
        var cacheKey = $"{ActiviteCacheKeyPrefix}{login}";

        if (_cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached != null)
        {
            return cached;
        }

        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var loginVariants = new List<string> { login };
            if (login.Contains('\\'))
            {
                loginVariants.Add(login.Split('\\').Last());
            }

            var utilisateur = dbContext.Set<Utilisateur>()
                .AsNoTracking()
                .FirstOrDefault(u => loginVariants.Contains(u.Login) && u.EstActif);

            if (utilisateur == null)
            {
                _logger.LogDebug("Utilisateur {Login} non trouvé pour activités", login);
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var activiteCodes = dbContext.Set<UtilisateurActivite>()
                .AsNoTracking()
                .Where(ua => ua.IdUtilisateur == utilisateur.Id && ua.EstActif)
                .Join(dbContext.Set<Activite>().Where(a => a.EstActif),
                    ua => ua.IdActivite,
                    a => a.Id,
                    (ua, a) => a.CodeActivite)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _logger.LogDebug("Activités DB pour {Login}: {Count} activités", login, activiteCodes.Count);

            _cache.Set(cacheKey, activiteCodes, CacheDuration);
            return activiteCodes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des activités DB pour {Login}", login);
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
