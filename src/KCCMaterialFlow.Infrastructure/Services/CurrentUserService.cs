using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;
using KCCMaterialFlow.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service pour accéder aux informations de l'utilisateur courant via Windows Authentication
/// Utilise un cache mémoire pour éviter les appels répétés à Active Directory
/// Supporte la simulation d'utilisateur en mode développement
/// </summary>
[SupportedOSPlatform("windows")]
public class CurrentUserService : ICurrentUserService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrentUserService> _logger;
    
    // Durée de cache pour les infos utilisateur (5 minutes)
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    
    // Simulation d'utilisateur (mode DEV uniquement)
    private static bool _isSimulationActive;
    private static string? _simulatedLogin;
    private static string? _simulatedDisplayName;
    private static string? _simulatedEmail;
    private static string? _simulatedDepartment;
    private static List<string> _simulatedRoles = new();

    public CurrentUserService(
        IIdentityProvider identityProvider,
        IMemoryCache cache,
        ILogger<CurrentUserService> logger)
    {
        _identityProvider = identityProvider;
        _cache = cache;
        _logger = logger;
    }
    
    /// <summary>
    /// Active la simulation d'un utilisateur spécifique (pour les administrateurs)
    /// </summary>
    public void SetSimulatedUser(string login, string displayName, string? email, string? department, IEnumerable<string> roles)
    {
        _simulatedLogin = login;
        _simulatedDisplayName = displayName;
        _simulatedEmail = email;
        _simulatedDepartment = department;
        _simulatedRoles = roles.ToList();
        _isSimulationActive = true;
    }

    /// <summary>
    /// Désactive la simulation d'utilisateur
    /// </summary>
    public void ClearSimulation()
    {
        _isSimulationActive = false;
        _simulatedLogin = null;
        _simulatedDisplayName = null;
        _simulatedEmail = null;
        _simulatedDepartment = null;
        _simulatedRoles.Clear();
    }

    /// <summary>
    /// Indique si la simulation est active
    /// </summary>
    public bool IsSimulationActive => _isSimulationActive;

    /// <summary>
    /// Login de l'utilisateur simulé
    /// </summary>
    public static string? SimulatedLogin => _simulatedLogin;

    /// <summary>
    /// Nom de l'utilisateur simulé
    /// </summary>
    public static string? SimulatedDisplayName => _simulatedDisplayName;

    /// <inheritdoc />
    public CurrentUserInfo? GetCurrentUser()
    {
        // En mode simulation (pour les administrateurs), retourner l'utilisateur simulé
        if (_isSimulationActive && !string.IsNullOrEmpty(_simulatedLogin))
        {
            return new CurrentUserInfo
            {
                Login = _simulatedLogin,
                DisplayName = _simulatedDisplayName ?? _simulatedLogin,
                Email = _simulatedEmail,
                Department = _simulatedDepartment,
                Roles = _simulatedRoles,
                IsActive = true
            };
        }

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

            var cacheKey = $"UserInfo_{login}";

            // Vérifier le cache d'abord
            if (_cache.TryGetValue(cacheKey, out CurrentUserInfo? cachedUser))
            {
                return cachedUser;
            }

            // Récupérer toutes les infos AD en un seul appel
            var userInfo = GetUserInfoFromAD(identity, login);
            
            // Mettre en cache
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
    /// Récupère toutes les informations utilisateur depuis AD en un seul appel
    /// </summary>
    private CurrentUserInfo GetUserInfoFromAD(WindowsIdentity identity, string login)
    {
        var userInfo = new CurrentUserInfo
        {
            Login = login,
            DisplayName = login,
            IsActive = true,
            Roles = new List<string> { "Demandeur" }
        };

        try
        {
            // Essayer d'abord avec le domaine extrait du login
            var domainName = login.Contains('\\') ? login.Split('\\')[0] : null;
            
            PrincipalContext? context = null;
            try
            {
                // Essayer avec le nom de domaine spécifique
                if (!string.IsNullOrEmpty(domainName))
                {
                    context = new PrincipalContext(ContextType.Domain, domainName);
                }
            }
            catch
            {
                // Si échec, essayer sans spécifier le domaine
                try
                {
                    context = new PrincipalContext(ContextType.Domain);
                }
                catch
                {
                    // Dernière tentative avec Machine
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

                        // Récupérer le département et la fonction via DirectoryEntry
                        if (user.GetUnderlyingObject() is System.DirectoryServices.DirectoryEntry de)
                        {
                            userInfo.Department = de.Properties["department"]?.Value?.ToString();
                            userInfo.Function = de.Properties["title"]?.Value?.ToString();
                        }
                        
                        _logger.LogInformation("Utilisateur AD trouvé: {DisplayName}", userInfo.DisplayName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de récupérer les infos AD pour {Login}, utilisation du fallback", login);
        }

        // Récupérer les rôles (séparé car utilise WindowsPrincipal)
        userInfo.Roles = GetRolesFromAD(identity).ToList();

        return userInfo;
    }

    /// <inheritdoc />
    public string GetUserLogin()
    {
        try
        {
            if (!_identityProvider.IsAuthenticated)
                return "Anonyme";
            
            // Capturer le nom de manière synchrone pour éviter ObjectDisposedException
            return _identityProvider.GetCurrentUserName() ?? "Anonyme";
        }
        catch (ObjectDisposedException)
        {
            // Le token Windows a été disposé - retourner une valeur par défaut
            _logger.LogWarning("WindowsIdentity disposée lors de GetUserLogin");
            return "Anonyme";
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
        catch (ObjectDisposedException)
        {
            _logger.LogWarning("WindowsIdentity disposée lors de GetUserDisplayName");
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
    /// <remarks>Implémentation de base - toujours true. La vérification réelle est dans DatabaseRoleEnricherService.</remarks>
    public bool HasActivite(string codeActivite) => true;

    /// <inheritdoc />
    /// <remarks>Implémentation de base - toujours true. La vérification réelle est dans DatabaseRoleEnricherService.</remarks>
    public bool HasAnyActivite(params string[] codeActivites) => true;

    /// <summary>
    /// Récupère les rôles applicatifs depuis les groupes AD
    /// Les groupes AD sont mappés aux rôles de l'application
    /// </summary>
    private IEnumerable<string> GetRolesFromAD(WindowsIdentity identity)
    {
        var roles = new List<string>();

        try
        {
            var principal = new WindowsPrincipal(identity);

            // Mapping des groupes AD vers les rôles applicatifs
            var roleMapping = new Dictionary<string, string>
            {
                { "KCC_MF_Admin", "Admin" },
                { "KCC_MF_Superviseur", "Superviseur" },
                { "KCC_MF_GM", "GM" },
                { "KCC_MF_IT", "IT" },
                { "KCC_MF_Environnement", "Environnement" },
                { "KCC_MF_OPJ", "OPJ" },
                { "KCC_MF_Identification", "Identification" },
                { "KCC_MF_Investigation", "Investigation" },
                { "KCC_MF_Security", "Security" },
                { "KCC_MF_Demandeur", "Demandeur" }
            };

            // Mode développement : Ajouter automatiquement le rôle Admin pour les développeurs locaux
            // IMPORTANT: À désactiver en production !
            #if DEBUG
            var devAdminLogins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // Ajouter ici les logins des développeurs qui doivent avoir accès Admin en local
                "CDKTGNBK01127\\ikasa",  // Login local dev
                "ikasa",                  // Sans domaine
                Environment.UserName      // Utilisateur Windows courant
            };
            
            if (devAdminLogins.Contains(identity.Name ?? "") || 
                devAdminLogins.Contains(identity.Name?.Split('\\').LastOrDefault() ?? ""))
            {
                roles.Add("Admin");
                _logger.LogWarning("Mode DEV: Rôle Admin attribué automatiquement à {Login}", identity.Name);
            }
            #endif

            foreach (var (adGroup, appRole) in roleMapping)
            {
                try
                {
                    if (principal.IsInRole(adGroup))
                    {
                        roles.Add(appRole);
                    }
                }
                catch
                {
                    // Ignorer si le groupe n'existe pas
                }
            }

            // Si aucun rôle trouvé, assigner le rôle par défaut "Demandeur"
            if (roles.Count == 0)
            {
                roles.Add("Demandeur");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur lors de la récupération des rôles AD");
            roles.Add("Demandeur");
        }

        return roles;
    }
}
