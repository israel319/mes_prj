namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Middleware HTTP qui vérifie si l'utilisateur authentifié existe et est actif en BD.
/// Utilise UserRoleCacheService comme source unique (même cache que ClaimsTransformation).
/// Les utilisateurs inexistants ou inactifs sont redirigés vers /access-denied.
/// </summary>
public class UserAccessMiddleware
{
    private readonly RequestDelegate _next;

    // Chemins techniques à laisser passer sans vérification
    private static readonly string[] TechnicalPaths =
    [
        "/_blazor",           // SignalR WebSocket pour Blazor Server
        "/_framework",        // Blazor framework files
        "/_content",          // Radzen et autres composants
        "/css",
        "/js",
        "/favicon",
        "/api/export",
        "/Error",             // Page d'erreur
        "/.well-known"        // HTTPS challenge
    ];

    public UserAccessMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserRoleCacheService roleCacheService)
    {
        var path = context.Request.Path.Value ?? "";

        // Laisser passer les chemins techniques
        if (TechnicalPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        // Laisser passer /access-denied lui-même
        if (path.Equals("/access-denied", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Ne vérifier que les utilisateurs authentifiés
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

        // Utiliser le cache centralisé : si pas de rôle → user inexistant ou inactif
        var role = await roleCacheService.GetUserRoleAsync(login);
        if (string.IsNullOrEmpty(role))
        {
            context.Response.Redirect("/access-denied");
            return;
        }

        await _next(context);
    }
}
