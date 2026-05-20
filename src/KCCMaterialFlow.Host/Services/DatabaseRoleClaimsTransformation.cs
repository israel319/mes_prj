using System.Security.Claims;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Injecte les claims dérivés de AppUser.NiveauAdmin dans le ClaimsPrincipal.
/// Source unique = T_Users (AppUser), lookup par Login Windows.
///
/// Claims injectés (identité "DatabaseRoles") :
///   - ClaimTypes.Role : "Admin" / "SuperAdmin" (si NiveauAdmin >= Admin)
///   - Rôles applicatifs legacy pour rétrocompatibilité [Authorize(Roles=...)]
/// </summary>
public class DatabaseRoleClaimsTransformation : IClaimsTransformation
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<DatabaseRoleClaimsTransformation> _logger;

    /// <summary>
    /// Rôles applicatifs legacy encore référencés par d'anciennes pages via [Authorize(Roles="...")].
    /// SuperAdmin et Admin les reçoivent automatiquement.
    /// </summary>
    private static readonly string[] LegacyApplicativeRoles =
    {
        "Superviseur", "GM", "IT", "Environnement",
        "OPJ", "Identification", "Barriere", "Investigation", "Security"
    };

    public DatabaseRoleClaimsTransformation(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<DatabaseRoleClaimsTransformation> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return principal;

        var login = identity.Name;
        if (string.IsNullOrEmpty(login))
            return principal;

        if (principal.HasClaim(c => c.Type == "DbRolesLoaded" && c.Value == "true"))
            return principal;

        NiveauAdmin niveauAdmin = NiveauAdmin.Aucun;
        try
        {
            await using var ctx = await _dbContextFactory.CreateDbContextAsync();
            var loginUpper = login.ToUpperInvariant();

            var appUser = await ctx.AppUsers
                .AsNoTracking()
                .Where(u => u.EstActif && u.Login.ToUpper() == loginUpper)
                .Select(u => (NiveauAdmin?)u.NiveauAdmin)
                .FirstOrDefaultAsync();

            // Fallback SAM : compare uniquement la partie après '\'
            if (appUser == null && login.Contains('\\'))
            {
                var sam = login[(login.LastIndexOf('\\') + 1)..].ToUpperInvariant();
                appUser = await ctx.AppUsers
                    .AsNoTracking()
                    .Where(u => u.EstActif && u.Login.ToUpper().EndsWith("\\" + sam))
                    .Select(u => (NiveauAdmin?)u.NiveauAdmin)
                    .FirstOrDefaultAsync();
            }

            if (appUser.HasValue)
                niveauAdmin = appUser.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lecture AppUser pour {Login}", login);
        }

        var claims = new List<Claim> { new("DbRolesLoaded", "true") };

        if (niveauAdmin >= NiveauAdmin.Admin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            foreach (var legacyRole in LegacyApplicativeRoles)
                claims.Add(new Claim(ClaimTypes.Role, legacyRole));
        }
        if (niveauAdmin == NiveauAdmin.SuperAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "SuperAdmin"));

        var newPrincipal = new ClaimsPrincipal();
        foreach (var existingIdentity in principal.Identities)
            newPrincipal.AddIdentity(existingIdentity);

        newPrincipal.AddIdentity(new ClaimsIdentity(claims, "DatabaseRoles", ClaimTypes.Name, ClaimTypes.Role));
        return newPrincipal;
    }
}