using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Handler d'autorisation qui vérifie les rôles via les Claims uniquement,
/// sans appeler WindowsPrincipal.IsInRole() qui interroge Active Directory.
/// Résout l'erreur "The trust relationship between this workstation and the primary domain failed".
/// </summary>
public class ClaimsRolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RolesAuthorizationRequirement requirement)
    {
        if (context.User == null)
            return Task.CompletedTask;

        // Vérifier les rôles uniquement via les Claims (pas via WindowsPrincipal.IsInRole)
        var userRoles = context.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (requirement.AllowedRoles.Any(role => userRoles.Contains(role)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
