using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service pour accéder aux informations de l'utilisateur courant via Windows Authentication.
/// Les RÔLES ne sont PAS lus depuis AD — ils viennent de la BD via UserRoleCacheService/DatabaseRoleEnricherService.
/// Ce service fournit uniquement les infos de profil AD (DisplayName, Email, Department).
/// </summary>
[SupportedOSPlatform("windows")]
public class CurrentUserService : ICurrentUserService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrentUserService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public CurrentUserService(
        IIdentityProvider identityProvider,
        IMemoryCache cache,
        ILogger<CurrentUserService> logger)
    {
        _identityProvider = identityProvider;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public CurrentUserInfo? GetCurrentUser()
    {
        try
        {
            var identity = _identityProvider.GetCurrentWindowsIdentity();
            if (identity == null || !identity.IsAuthenticated)
                return null;

            string login;
            try
            {
                login = identity.Name ?? string.Empty;
            }
            catch (ObjectDisposedException)
            {
                _logger.LogWarning("WindowsIdentity disposée lors de GetCurrentUser");
                return null;
            }

            if (string.IsNullOrEmpty(login))
                return null;

            var cacheKey = $"UserProfile_{login}";

            if (_cache.TryGetValue(cacheKey, out CurrentUserInfo? cachedUser))
                return cachedUser;

            // Récupérer les infos de profil AD (pas les rôles)
            var userInfo = GetProfileFromAD(identity, login);

            _cache.Set(cacheKey, userInfo, CacheDuration);
            return userInfo;
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogWarning(ex, "WindowsIdentity disposée");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur courant");
            return null;
        }
    }

    /// <summary>
    /// Récupère les informations de profil depuis AD (DisplayName, Email, Department).
    /// NE récupère PAS les rôles — ceux-ci viennent de la BD.
    /// </summary>
    private CurrentUserInfo GetProfileFromAD(WindowsIdentity identity, string login)
    {
        var userInfo = new CurrentUserInfo
        {
            Login = login,
            DisplayName = login,
            IsActive = true,
            Roles = new List<string>() // Rempli par DatabaseRoleEnricherService
        };

        try
        {
            var domainName = login.Contains('\\') ? login.Split('\\')[0] : null;

            PrincipalContext? context = null;
            try
            {
                if (!string.IsNullOrEmpty(domainName))
                {
                    context = new PrincipalContext(ContextType.Domain, domainName);
                }
            }
            catch
            {
                try
                {
                    context = new PrincipalContext(ContextType.Domain);
                }
                catch
                {
                    context = new PrincipalContext(ContextType.Machine);
                }
            }

            if (context != null)
            {
                using (context)
                {
                    var samAccountName = login.Contains('\\') ? login.Split('\\')[1] : login;
                    using var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samAccountName);

                    if (user != null)
                    {
                        userInfo.DisplayName = user.DisplayName ?? user.Name ?? login;
                        userInfo.Email = user.EmailAddress;

                        if (user.GetUnderlyingObject() is System.DirectoryServices.DirectoryEntry de)
                        {
                            userInfo.Department = de.Properties["department"]?.Value?.ToString();
                            userInfo.Function = de.Properties["title"]?.Value?.ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de récupérer les infos AD pour {Login}, utilisation du login comme DisplayName", login);
        }

        return userInfo;
    }

    /// <inheritdoc />
    public string GetUserLogin()
    {
        try
        {
            if (!_identityProvider.IsAuthenticated)
                return "Anonyme";

            return _identityProvider.GetCurrentUserName() ?? "Anonyme";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de GetUserLogin");
            return "Anonyme";
        }
    }

    /// <inheritdoc />
    public string GetUserDisplayName()
    {
        try
        {
            var user = GetCurrentUser();
            return user?.DisplayName ?? GetUserLogin();
        }
        catch (Exception)
        {
            return "Anonyme";
        }
    }

    /// <inheritdoc />
    public string? GetUserEmail()
    {
        var user = GetCurrentUser();
        return user?.Email;
    }

    /// <inheritdoc />
    public string? GetUserDepartment()
    {
        var user = GetCurrentUser();
        return user?.Department;
    }

    /// <inheritdoc />
    public bool IsInRole(string role)
    {
        var roles = GetUserRoles();
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public bool IsInAnyRole(params string[] roles)
    {
        var userRoles = GetUserRoles();
        return roles.Any(r => userRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public IEnumerable<string> GetUserRoles()
    {
        var user = GetCurrentUser();
        return user?.Roles ?? Enumerable.Empty<string>();
    }

    /// <inheritdoc />
    public bool IsAuthenticated()
    {
        return _identityProvider.IsAuthenticated;
    }

    /// <inheritdoc />
    /// <remarks>Implémentation de base — toujours true. La vraie vérification est dans DatabaseRoleEnricherService.</remarks>
    public bool HasActivite(string codeActivite) => true;

    /// <inheritdoc />
    /// <remarks>Implémentation de base — toujours true. La vraie vérification est dans DatabaseRoleEnricherService.</remarks>
    public bool HasAnyActivite(params string[] codeActivites) => true;
}
