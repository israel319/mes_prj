using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Injecte les rôles de la base de données dans le ClaimsPrincipal.
/// Utilise UserRoleCacheService comme source unique.
/// Si l'utilisateur n'est pas en BD ou est inactif → PAS de claim Role → [Authorize] le bloquera.
/// </summary>
public class DatabaseRoleClaimsTransformation : IClaimsTransformation
{
    private readonly UserRoleCacheService _roleCacheService;
    private readonly ILogger<DatabaseRoleClaimsTransformation> _logger;

    public DatabaseRoleClaimsTransformation(
        UserRoleCacheService roleCacheService,
        ILogger<DatabaseRoleClaimsTransformation> logger)
    {
        _roleCacheService = roleCacheService;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return principal;

        var login = identity.Name;
        if (string.IsNullOrEmpty(login))
            return principal;

        // Éviter la double injection si déjà traité
        if (principal.HasClaim(c => c.Type == "DbRolesLoaded" && c.Value == "true"))
            return principal;

        var role = await _roleCacheService.GetUserRoleAsync(login);

        var newPrincipal = new ClaimsPrincipal();
        foreach (var existingIdentity in principal.Identities)
        {
            newPrincipal.AddIdentity(existingIdentity);
        }

        var roleClaims = new List<Claim> { new("DbRolesLoaded", "true") };

        if (!string.IsNullOrEmpty(role))
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        // PAS de fallback "Demandeur" — si pas en BD, pas de rôle = accès refusé par [Authorize]

        var dbIdentity = new ClaimsIdentity(roleClaims, "DatabaseRoles", ClaimTypes.Name, ClaimTypes.Role);
        newPrincipal.AddIdentity(dbIdentity);

        return newPrincipal;
    }
}
