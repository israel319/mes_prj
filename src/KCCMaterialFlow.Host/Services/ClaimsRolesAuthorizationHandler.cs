using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Handler d'autorisation qui vérifie les rôles via UserRoleCacheService (cache centralisé BD),
/// PAS via les Claims statiques du ClaimsPrincipal (qui sont figées pour le circuit Blazor).
/// Cela permet un effet IMMÉDIAT lorsqu'un admin change le rôle d'un utilisateur,
/// sans nécessiter un rechargement complet de la page.
/// </summary>
public class ClaimsRolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
{
    private readonly UserRoleCacheService _roleCacheService;

    public ClaimsRolesAuthorizationHandler(UserRoleCacheService roleCacheService)
    {
        _roleCacheService = roleCacheService;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RolesAuthorizationRequirement requirement)
    {
        if (context.User?.Identity is not { IsAuthenticated: true })
            return Task.CompletedTask;

        var login = context.User.Identity.Name;
        if (string.IsNullOrEmpty(login))
            return Task.CompletedTask;

        // Lire le rôle FRAIS depuis le cache centralisé (pas les Claims figées)
        var currentRole = _roleCacheService.GetUserRole(login);

        if (!string.IsNullOrEmpty(currentRole) &&
            requirement.AllowedRoles.Any(r => r.Equals(currentRole, StringComparison.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
