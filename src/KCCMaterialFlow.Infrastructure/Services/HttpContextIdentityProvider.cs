using System.Runtime.Versioning;
using System.Security.Principal;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Implémentation de IIdentityProvider utilisant HttpContext pour Blazor Server.
/// IMPORTANT : En Blazor Server, HttpContext n'existe que pendant la requête HTTP initiale.
/// Cette classe capture et met en cache les données utilisateur dès le premier accès,
/// puis les sert depuis le cache pour toute la durée du circuit (scoped DI).
/// Cela évite les ObjectDisposedException qui causent des boucles de reconnexion.
/// </summary>
[SupportedOSPlatform("windows")]
public class HttpContextIdentityProvider : IIdentityProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpContextIdentityProvider> _logger;

    // Cache capturé une seule fois lors du premier accès (pendant la requête HTTP initiale)
    private bool _captured;
    private string? _cachedUserName;
    private bool _cachedIsAuthenticated;
    private WindowsIdentity? _cachedIdentity;

    public HttpContextIdentityProvider(
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpContextIdentityProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Capture les données utilisateur depuis HttpContext la première fois,
    /// puis sert depuis le cache pour le reste du circuit Blazor.
    /// </summary>
    private void EnsureCaptured()
    {
        if (_captured) return;
        _captured = true;

        try
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity;
            if (identity is { IsAuthenticated: true })
            {
                _cachedIsAuthenticated = true;
                _cachedUserName = identity.Name;

                // Cloner le WindowsIdentity pour survivre à la fin de la requête HTTP
                if (identity is WindowsIdentity winIdentity)
                {
                    try
                    {
                        _cachedIdentity = (WindowsIdentity)winIdentity.Clone();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossible de cloner WindowsIdentity, les requêtes AD seront en mode dégradé");
                    }
                }
            }
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogWarning(ex, "HttpContext/Identity déjà disposé lors de la capture initiale");
        }
    }

    /// <inheritdoc />
    public WindowsIdentity? GetCurrentWindowsIdentity()
    {
        EnsureCaptured();
        return _cachedIdentity;
    }

    /// <inheritdoc />
    public string? GetCurrentUserName()
    {
        EnsureCaptured();
        return _cachedUserName;
    }

    /// <inheritdoc />
    public bool IsAuthenticated
    {
        get
        {
            EnsureCaptured();
            return _cachedIsAuthenticated;
        }
    }
}
