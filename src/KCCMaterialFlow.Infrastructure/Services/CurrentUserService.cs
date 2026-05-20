using System.Runtime.Versioning;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service de base : résout l'utilisateur courant via Windows Authentication
/// puis fait un lookup BD sur T_Employees par Matricule (extrait du login Windows)
/// et sur T_Users (AppUser) pour NiveauAdmin/EstActif.
/// </summary>
/// <remarks>
/// L'Employee porte le profil RH (DisplayName, Email, Departement).
/// L'AppUser porte les droits applicatifs (NiveauAdmin, EstActif).
/// </remarks>
[SupportedOSPlatform("windows")]
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrentUserService> _logger;
    private readonly IImpersonationService _impersonation;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<CurrentUserService> logger,
        IImpersonationService impersonation)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
        _impersonation = impersonation;
    }

    // ── Identité brute ─────────────────────────────────────────────

    public string GetUserLogin()
    {
        // Si une impersonation est active avec un login Windows connu, utiliser ce login
        // pour que toutes les résolutions (AppUser, Employee, NiveauAdmin) soient celles de l'impersonné.
        if (_impersonation.IsImpersonating && !string.IsNullOrEmpty(_impersonation.ImpersonatedLogin))
            return _impersonation.ImpersonatedLogin;

        try
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity;
            if (identity is not { IsAuthenticated: true })
                return "Anonyme";
            return identity.Name ?? "Anonyme";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetUserLogin");
            return "Anonyme";
        }
    }

    public bool IsAuthenticated() =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Extrait le matricule depuis le login Windows : DOMAIN\K26561 → K26561.
    /// </summary>
    public string? Matricule => ExtractMatricule(GetUserLogin());

    internal static string? ExtractMatricule(string? login)
    {
        if (string.IsNullOrWhiteSpace(login) || login == "Anonyme") return null;
        var afterSlash = login.Contains('\\') ? login[(login.LastIndexOf('\\') + 1)..] : login;
        var afterAt = afterSlash.Contains('@') ? afterSlash[..afterSlash.IndexOf('@')] : afterSlash;
        return string.IsNullOrWhiteSpace(afterAt) ? null : afterAt.Trim().ToUpperInvariant();
    }

    // ── Lookup Employee (cache 30 s) ───────────────────────────────

    public Employee? GetCurrentEmployee()
    {
        // Login Windows → T_Users.Login → AppUser.EmployeeId → T_Employees.Id
        var appUser = GetCurrentAppUser();
        if (appUser != null && appUser.EmployeeId.HasValue)
        {
            var cacheKey = $"Employee_ById_{appUser.EmployeeId.Value}";
            if (_cache.TryGetValue(cacheKey, out Employee? cached)) return cached;

            try
            {
                using var ctx = _dbContextFactory.CreateDbContext();
                var emp = ctx.Employees.AsNoTracking().FirstOrDefault(e => e.Id == appUser.EmployeeId.Value);
                if (emp != null) _cache.Set(cacheKey, emp, CacheDuration);
                return emp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lookup Employee par Id {Id}", appUser.EmployeeId.Value);
                return null;
            }
        }

        // Fallback impersonation : user standalone sans AppUser — lookup par EmployeeCode
        if (_impersonation.IsImpersonating && !string.IsNullOrEmpty(_impersonation.ImpersonatedMatricule))
        {
            var code = _impersonation.ImpersonatedMatricule;
            var cacheKey = $"Employee_ByCode_{code.ToUpperInvariant()}";
            if (_cache.TryGetValue(cacheKey, out Employee? cached)) return cached;

            try
            {
                using var ctx = _dbContextFactory.CreateDbContext();
                var emp = ctx.Employees.AsNoTracking()
                    .FirstOrDefault(e => e.Matricule != null && e.Matricule.ToUpper() == code.ToUpperInvariant());
                if (emp != null) _cache.Set(cacheKey, emp, CacheDuration);
                return emp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lookup Employee par code {Code}", code);
                return null;
            }
        }

        return null;
    }

    // ── Lookup AppUser (cache 30 s) ────────────────────────────────

    public AppUser? GetCurrentAppUser()
    {
        var login = GetUserLogin();
        if (string.IsNullOrWhiteSpace(login) || login == "Anonyme") return null;

        var cacheKey = $"AppUser_ByLogin_{login.ToUpperInvariant()}";
        if (_cache.TryGetValue(cacheKey, out AppUser? cached)) return cached;

        try
        {
            using var ctx = _dbContextFactory.CreateDbContext();
            var loginUpper = login.ToUpperInvariant();
            var appUser = ctx.AppUsers.AsNoTracking()
                .FirstOrDefault(u => u.EstActif && u.Login.ToUpper() == loginUpper);

            // Fallback SAM : DOMAIN\K26561 → K26561
            if (appUser == null && login.Contains('\\'))
            {
                var sam = login[(login.LastIndexOf('\\') + 1)..].ToUpperInvariant();
                appUser = ctx.AppUsers.AsNoTracking()
                    .FirstOrDefault(u => u.EstActif && u.Login.ToUpper().EndsWith("\\" + sam));
            }

            if (appUser != null)
                _cache.Set(cacheKey, appUser, CacheDuration);
            return appUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lookup AppUser par login {Login}", login);
            return null;
        }
    }

    // ── Niveau admin ───────────────────────────────────────────────

    public NiveauAdmin NiveauAdmin => GetCurrentAppUser()?.NiveauAdmin ?? NiveauAdmin.Aucun;
    public bool IsAdmin => NiveauAdmin >= NiveauAdmin.Admin;
    public bool IsSuperAdmin => NiveauAdmin == NiveauAdmin.SuperAdmin;
    public int? EmployeeId => GetCurrentEmployee()?.Id;

    public bool IsApprover
    {
        get
        {
            if (IsAdmin) return true;
            var emp = GetCurrentEmployee();
            if (emp == null) return false;
            var cacheKey = $"IsApprover_{emp.Id}";
            if (_cache.TryGetValue(cacheKey, out bool cached)) return cached;
            try
            {
                using var ctx = _dbContextFactory.CreateDbContext();
                var result =
                    ctx.Employees.AsNoTracking().Any(e => e.ReportToEmployeeId == emp.Id) ||
                    ctx.Set<WorkflowApprobateurSpecial>().AsNoTracking().Any(w => w.EmployeeId == emp.Id && w.EstActif);
                _cache.Set(cacheKey, result, CacheDuration);
                return result;
            }
            catch { return false; }
        }
    }

    // ── Profil pour affichage ──────────────────────────────────────

    public string GetUserDisplayName()
    {
        var emp = GetCurrentEmployee();
        if (emp != null && !string.IsNullOrWhiteSpace(emp.DisplayName))
            return emp.DisplayName;
        if (emp != null && !string.IsNullOrWhiteSpace(emp.NomComplet))
            return emp.NomComplet;
        return Matricule ?? GetUserLogin();
    }

    public string GetUserFirstName()
    {
        var emp = GetCurrentEmployee();
        if (emp != null && !string.IsNullOrWhiteSpace(emp.Prenom))
            return emp.Prenom!.Trim();
        var raw = emp?.DisplayName ?? emp?.NomComplet ?? string.Empty;
        // DisplayName est généralement "Matricule - PRENOM NOM" → on enlève le préfixe matricule
        var idx = raw.IndexOf(" - ", StringComparison.Ordinal);
        if (idx >= 0) raw = raw[(idx + 3)..];
        var first = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(first) ? (Matricule ?? GetUserLogin()) : first!;
    }

    public string? GetUserEmail() => GetCurrentEmployee()?.Email;
    public string? GetUserDepartment() => GetCurrentEmployee()?.DepartementNom;

    // ── CurrentUserInfo (snapshot rétrocompatible) ─────────────────

    public CurrentUserInfo? GetCurrentUser()
    {
        if (!IsAuthenticated()) return null;

        var login = GetUserLogin();
        var matricule = Matricule;
        var emp = GetCurrentEmployee();
        var appUser = GetCurrentAppUser();

        return new CurrentUserInfo
        {
            UserId = emp?.Id ?? 0,
            Login = login,
            Matricule = matricule,
            DisplayName = GetUserDisplayName(),
            Email = emp?.Email,
            Function = emp?.Fonction,
            Department = emp?.DepartementNom,
            DepartmentId = null,
            NiveauAdmin = appUser?.NiveauAdmin ?? NiveauAdmin.Aucun,
            IsActive = appUser?.EstActif ?? false,
            Roles = new List<string>()
        };
    }

    // ── STUBS LEGACY ──────────────────────────────────────────────
#pragma warning disable CS0618
    public bool IsInRole(string role)
    {
        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)) return IsAdmin;
        if (string.Equals(role, "SuperAdmin", StringComparison.OrdinalIgnoreCase)) return IsSuperAdmin;
        return false;
    }

    public bool IsInAnyRole(params string[] roles) => roles?.Any(IsInRole) ?? false;

    public IEnumerable<string> GetUserRoles()
    {
        if (IsSuperAdmin) return new[] { "SuperAdmin", "Admin" };
        if (IsAdmin) return new[] { "Admin" };
        return Enumerable.Empty<string>();
    }

    public bool HasActivite(string codeActivite) => IsAuthenticated();

    public bool HasAnyActivite(params string[] codeActivites) => IsAuthenticated();
#pragma warning restore CS0618

    // ── Auto-création depuis Glencore ──────────────────────────────
    /// <summary>
    /// Cherche un GlencoreEmployee par UserName (DOMAIN\sam ou sam) et auto-crée
    /// un Employee local correspondant. Permet à tout utilisateur présent dans le
    /// référentiel Glencore de se connecter sans configuration manuelle préalable.
    /// </summary>
    private Employee? TryAutoCreateFromGlencore(KCCMaterialFlowDbContext ctx, string login)
    {
        try
        {
            var loginLower = login.ToLowerInvariant();
            var sam = login.Contains('\\') ? login[(login.LastIndexOf('\\') + 1)..] : login;
            var samLower = sam.ToLowerInvariant();

            var glencore = ctx.AllEmployees
                .AsNoTracking()
                .FirstOrDefault(g => g.UserName != null && (
                    g.UserName.ToLower() == loginLower ||
                    g.UserName.ToLower().EndsWith("\\" + samLower) ||
                    g.UserName.ToLower() == samLower));

            if (glencore == null)
            {
                _logger.LogWarning("Login '{Login}' introuvable dans T_AllEmployees — auto-création impossible.", login);
                return null;
            }

            var displayName = $"{glencore.FirstName} {glencore.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(displayName)) displayName = glencore.EmployeeCode;

            // Vérifier qu'il n'existe pas déjà un Employee avec ce matricule (course, race condition)
            var existing = ctx.Employees.FirstOrDefault(e => e.Matricule == glencore.EmployeeCode);
            if (existing != null) return existing;

            var stub = new Employee
            {
                Matricule = glencore.EmployeeCode,
                NomComplet = displayName,
                DisplayName = displayName,
                Prenom = glencore.FirstName,
                Nom = glencore.LastName,
                Email = glencore.Mail,
                DepartementNom = glencore.Departement,
                EstInterne = true,
                DateCreation = DateTime.Now
            };

            ctx.Employees.Add(stub);
            ctx.SaveChanges();

            _logger.LogInformation(
                "Employee local auto-créé au login pour Glencore {EmployeeCode} ({Display}) — Id={Id}",
                glencore.EmployeeCode, displayName, stub.Id);

            return stub;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur auto-création Employee depuis Glencore pour login {Login}", login);
            return null;
        }
    }
}
