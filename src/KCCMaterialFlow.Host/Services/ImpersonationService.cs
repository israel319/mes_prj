using KCCMaterialFlow.Application.Common.Interfaces;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Lit l'état d'impersonation depuis les cookies HTTP posés par les endpoints
/// /api/impersonation/start (POST) et /api/impersonation/stop (POST).
///
/// Cookies attendus (HttpOnly, SameSite=Strict) :
///   imp_active    = "1"
///   imp_matricule = EmployeeCode impersonné
///   imp_reallogin = Login Windows réel (ex. ANYACCESS\ikasa)
/// </summary>
public sealed class ImpersonationService(IHttpContextAccessor httpContextAccessor)
    : IImpersonationService
{
    private const string CookieActive    = "imp_active";
    private const string CookieMatricule = "imp_matricule";
    private const string CookieLogin     = "imp_login";
    private const string CookieRealLogin = "imp_reallogin";

    private IRequestCookieCollection? Cookies =>
        httpContextAccessor.HttpContext?.Request.Cookies;

    public bool IsImpersonating =>
        Cookies?.TryGetValue(CookieActive, out var v) == true && v == "1";

    public string? ImpersonatedMatricule =>
        IsImpersonating && Cookies!.TryGetValue(CookieMatricule, out var m) ? m : null;

    public string? ImpersonatedLogin =>
        IsImpersonating && Cookies!.TryGetValue(CookieLogin, out var l) && !string.IsNullOrEmpty(l) ? l : null;

    public string? RealLogin =>
        IsImpersonating && Cookies!.TryGetValue(CookieRealLogin, out var r) ? r : null;
}
