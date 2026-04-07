using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Décorateur de ICurrentUserService qui enrichit avec les rôles BD via UserRoleCacheService.
/// Les activités sont dérivées automatiquement des rôles (via ActiviteService.RoleDefaultActivites).
/// Plus de cache local — tout passe par UserRoleCacheService (cache centralisé + invalidation).
/// </summary>
public class DatabaseRoleEnricherService : ICurrentUserService
{
    private readonly ICurrentUserService _innerService;
    private readonly UserRoleCacheService _roleCacheService;
    private readonly IActiviteService _activiteService;
    private readonly ILogger<DatabaseRoleEnricherService> _logger;

    public DatabaseRoleEnricherService(
        ICurrentUserService innerService,
        UserRoleCacheService roleCacheService,
        IActiviteService activiteService,
        ILogger<DatabaseRoleEnricherService> logger)
    {
        _innerService = innerService;
        _roleCacheService = roleCacheService;
        _activiteService = activiteService;
        _logger = logger;
    }

    public CurrentUserInfo? GetCurrentUser()
    {
        var user = _innerService.GetCurrentUser();
        if (user == null) return null;

        // BD = source unique pour les rôles (pas AD)
        var role = _roleCacheService.GetUserRole(user.Login);
        user.Roles = string.IsNullOrEmpty(role) ? new List<string>() : new List<string> { role };

        return user;
    }

    public string GetUserLogin() => _innerService.GetUserLogin();

    public string GetUserDisplayName() => _innerService.GetUserDisplayName();

    public string? GetUserEmail() => _innerService.GetUserEmail();

    public string? GetUserDepartment() => _innerService.GetUserDepartment();

    public bool IsAuthenticated() => _innerService.IsAuthenticated();

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
        var user = GetCurrentUser();
        return user?.Roles ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Vérifie si l'utilisateur a une activité donnée.
    /// Activités DÉRIVÉES des rôles via le mapping statique RoleDefaultActivites.
    /// </summary>
    public bool HasActivite(string codeActivite)
    {
        var roles = GetUserRoles();
        return roles.Any(role =>
        {
            var activites = _activiteService.GetDefaultActiviteCodesForRole(role);
            return activites.Contains(codeActivite, StringComparer.OrdinalIgnoreCase);
        });
    }

    /// <summary>
    /// Vérifie si l'utilisateur a au moins une des activités données.
    /// </summary>
    public bool HasAnyActivite(params string[] codeActivites)
    {
        var roles = GetUserRoles();
        var userActivites = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var role in roles)
        {
            var defaults = _activiteService.GetDefaultActiviteCodesForRole(role);
            foreach (var act in defaults)
                userActivites.Add(act);
        }

        return codeActivites.Any(code => userActivites.Contains(code));
    }
}
