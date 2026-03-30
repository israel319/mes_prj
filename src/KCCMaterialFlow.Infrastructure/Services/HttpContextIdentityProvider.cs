using System.Runtime.Versioning;
using System.Security.Principal;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Implémentation de IIdentityProvider utilisant HttpContext pour Blazor Server.
/// Cette classe encapsule l'accès à IHttpContextAccessor pour découpler la couche Core du Web.
/// </summary>
[SupportedOSPlatform("windows")]
public class HttpContextIdentityProvider : IIdentityProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpContextIdentityProvider> _logger;

    public HttpContextIdentityProvider(
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpContextIdentityProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public WindowsIdentity? GetCurrentWindowsIdentity()
    {
        try
        {
            return _httpContextAccessor.HttpContext?.User?.Identity as WindowsIdentity;
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogWarning(ex, "WindowsIdentity disposée lors de GetCurrentWindowsIdentity");
            return null;
        }
    }

    /// <inheritdoc />
    public string? GetCurrentUserName()
    {
        try
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity;
            if (identity == null || !identity.IsAuthenticated)
                return null;

            return identity.Name;
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogWarning(ex, "Identity disposée lors de GetCurrentUserName");
            return null;
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated
    {
        get
        {
            try
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }
}
