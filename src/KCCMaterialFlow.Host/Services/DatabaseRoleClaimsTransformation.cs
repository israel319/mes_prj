using System.Security.Claims;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Injecte les rôles de la base de données dans le ClaimsPrincipal.
/// Cela permet à [Authorize(Roles = "...")] de fonctionner sans interroger Active Directory,
/// évitant ainsi les erreurs "trust relationship between this workstation and the primary domain failed".
/// </summary>
public class DatabaseRoleClaimsTransformation : IClaimsTransformation
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly DevRoleSwitcherService _devRoleSwitcher;
    private readonly ILogger<DatabaseRoleClaimsTransformation> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string CacheKeyPrefix = "ClaimsRoles_";

    public DatabaseRoleClaimsTransformation(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        DevRoleSwitcherService devRoleSwitcher,
        ILogger<DatabaseRoleClaimsTransformation> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _devRoleSwitcher = devRoleSwitcher;
        _logger = logger;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        var login = identity.Name;
        if (string.IsNullOrEmpty(login))
            return Task.FromResult(principal);

        // Vérifier si les rôles DB ont déjà été injectés (éviter les doublons)
        if (principal.HasClaim(c => c.Type == "DbRolesLoaded" && c.Value == "true"))
            return Task.FromResult(principal);

        try
        {
            List<string> roles;

#if DEBUG
            // En mode dev avec simulation active, utiliser les rôles simulés
            if (_devRoleSwitcher.IsSimulationActive)
            {
                roles = _devRoleSwitcher.SimulatedRoles.ToList();
                _logger.LogDebug("DevRoleSwitcher actif - Rôles simulés: {Roles}", string.Join(", ", roles));
            }
            else
            {
                roles = GetDatabaseRoles(login);
            }
#else
            roles = GetDatabaseRoles(login);
#endif
            
            // CRITIQUE : On retourne un NOUVEAU ClaimsPrincipal (pas le WindowsPrincipal original).
            // WindowsPrincipal.IsInRole(string) appelle l'API Windows pour vérifier les groupes AD,
            // ce qui lance Win32Exception si le domaine n'est pas accessible.
            // Un simple ClaimsPrincipal.IsInRole() ne vérifie que les claims → pas d'appel AD.
            var newPrincipal = new ClaimsPrincipal();

            // Copier les identités existantes (y compris WindowsIdentity pour garder les infos de connexion)
            foreach (var existingIdentity in principal.Identities)
            {
                newPrincipal.AddIdentity(existingIdentity);
            }

            // Construire les claims de rôles DB
            var roleClaims = new List<Claim> { new("DbRolesLoaded", "true") };
            
            if (roles.Count > 0)
            {
                foreach (var role in roles)
                {
                    roleClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                _logger.LogDebug("Rôles DB injectés pour {Login}: {Roles}", login, string.Join(", ", roles));
            }
            else
            {
                // Aucun rôle en base → Demandeur par défaut
                roleClaims.Add(new Claim(ClaimTypes.Role, "Demandeur"));
            }

            // Ajouter une identité additionnelle avec les rôles DB
            var dbIdentity = new ClaimsIdentity(roleClaims, "DatabaseRoles", ClaimTypes.Name, ClaimTypes.Role);
            newPrincipal.AddIdentity(dbIdentity);

            return Task.FromResult(newPrincipal);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de charger les rôles DB pour {Login}, rôle Demandeur par défaut", login);
            
            // Même en erreur, retourner un nouveau ClaimsPrincipal pour éviter WindowsPrincipal.IsInRole
            var fallbackPrincipal = new ClaimsPrincipal();
            foreach (var existingIdentity in principal.Identities)
            {
                fallbackPrincipal.AddIdentity(existingIdentity);
            }
            var fallbackClaims = new List<Claim>
            {
                new("DbRolesLoaded", "true"),
                new(ClaimTypes.Role, "Demandeur")
            };
            var fallbackIdentity = new ClaimsIdentity(fallbackClaims, "DatabaseRoles", ClaimTypes.Name, ClaimTypes.Role);
            fallbackPrincipal.AddIdentity(fallbackIdentity);

            return Task.FromResult(fallbackPrincipal);
        }
    }

    private List<string> GetDatabaseRoles(string login)
    {
        var cacheKey = $"{CacheKeyPrefix}{login}";

        if (_cache.TryGetValue(cacheKey, out List<string>? cachedRoles) && cachedRoles != null)
            return cachedRoles;

        using var dbContext = _dbContextFactory.CreateDbContext();

        // Chercher l'utilisateur par son login (avec ou sans domaine)
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
            _logger.LogDebug("Utilisateur {Login} non trouvé en BD, pas de rôles DB", login);
            _cache.Set(cacheKey, new List<string>(), CacheDuration);
            return [];
        }

        var roles = dbContext.Set<UtilisateurRole>()
            .AsNoTracking()
            .Where(ur => ur.IdUtilisateur == utilisateur.IdUtilisateur)
            .Join(dbContext.Set<Role>().Where(r => r.EstActif),
                ur => ur.IdRole,
                r => r.IdRole,
                (ur, r) => r.CodeRole)
            .ToList();

        // Ajouter le rôle principal via la FK IdRole
        var directRole = dbContext.Set<Role>()
            .AsNoTracking()
            .Where(r => r.IdRole == utilisateur.IdRole && r.EstActif)
            .Select(r => r.CodeRole)
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(directRole) &&
            !roles.Contains(directRole, StringComparer.OrdinalIgnoreCase))
        {
            roles.Add(directRole);
        }

        _cache.Set(cacheKey, roles, CacheDuration);
        return roles;
    }
}
