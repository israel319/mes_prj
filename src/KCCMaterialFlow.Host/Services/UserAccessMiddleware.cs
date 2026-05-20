using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Middleware HTTP qui vérifie qu'un utilisateur authentifié possède un AppUser actif en BD.
/// Source unique = T_Users (AppUser), par Login Windows.
/// Les utilisateurs sans AppUser actif sont redirigés vers /access-denied.
/// </summary>
public class UserAccessMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly string[] TechnicalPaths =
    [
        "/_blazor",
        "/_framework",
        "/_content",
        "/css",
        "/js",
        "/favicon",
        "/api/export",
        "/Error",
        "/.well-known"
    ];

    public UserAccessMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory)
    {
        var path = context.Request.Path.Value ?? "";

        if (TechnicalPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        if (path.Equals("/access-denied", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var user = context.User;
        if (user.Identity is not { IsAuthenticated: true })
        {
            await _next(context);
            return;
        }

        var login = user.Identity.Name;
        if (string.IsNullOrEmpty(login))
        {
            await _next(context);
            return;
        }

        // Vérifier qu'un AppUser actif existe pour ce login
        try
        {
            await using var ctx = await dbContextFactory.CreateDbContextAsync();
            var loginUpper = login.ToUpperInvariant();

            var hasActiveUser = await ctx.AppUsers
                .AsNoTracking()
                .AnyAsync(u => u.EstActif && u.Login.ToUpper() == loginUpper);

            // Fallback SAM (développement cross-domain)
            if (!hasActiveUser && login.Contains('\\'))
            {
                var sam = login[(login.LastIndexOf('\\') + 1)..].ToUpperInvariant();
                hasActiveUser = await ctx.AppUsers
                    .AsNoTracking()
                    .AnyAsync(u => u.EstActif && u.Login.ToUpper().EndsWith("\\" + sam));
            }

            if (!hasActiveUser)
            {
                context.Response.Redirect("/access-denied");
                return;
            }
        }
        catch
        {
            // En cas d'erreur BD, laisser passer (ne pas bloquer l'accès sur erreur transitoire)
        }

        await _next(context);
    }
}