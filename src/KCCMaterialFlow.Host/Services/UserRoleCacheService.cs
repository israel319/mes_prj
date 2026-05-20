using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Cache de rôles utilisateur basé sur AppUser.NiveauAdmin (T_Users).
/// Remplace l'ancienne version basée sur Employee.NiveauAdmin (colonnes supprimées).
/// Enregistré Singleton — utilise IDbContextFactory + IMemoryCache (thread-safe).
/// </summary>
public sealed class UserRoleCacheService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    public UserRoleCacheService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    // ── API publique ────────────────────────────────────────────────────

    /// <summary>
    /// Extrait le SAM / matricule depuis un login Windows : DOMAIN\K26561 → K26561.
    /// Méthode statique pour rétrocompatibilité avec les appels directs dans les razors.
    /// </summary>
    public static string? ExtractMatricule(string? login)
    {
        if (string.IsNullOrWhiteSpace(login)) return null;
        var afterSlash = login.Contains('\\') ? login[(login.LastIndexOf('\\') + 1)..] : login;
        var afterAt = afterSlash.Contains('@') ? afterSlash[..afterSlash.IndexOf('@')] : afterSlash;
        return string.IsNullOrWhiteSpace(afterAt) ? null : afterAt.Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Retourne le rôle texte de l'utilisateur ("SuperAdmin", "Admin") ou null si aucun rôle applicatif.
    /// </summary>
    public string? GetUserRole(string? login)
    {
        return GetNiveauAdmin(login) switch
        {
            NiveauAdmin.SuperAdmin => "SuperAdmin",
            NiveauAdmin.Admin      => "Admin",
            _                      => null
        };
    }

    /// <summary>
    /// Indique si l'utilisateur est approbateur (NiveauAdmin >= Admin,
    /// ou désigné comme approbateur dans WorkflowApprobateursSpeciaux,
    /// ou a des employés qui lui rapportent).
    /// </summary>
    public bool IsApprover(string? login)
    {
        if (string.IsNullOrWhiteSpace(login)) return false;

        var niveau = GetNiveauAdmin(login);
        if (niveau >= NiveauAdmin.Admin) return true;

        // Vérifie via EmployeeId de l'AppUser
        var empId = GetEmployeeId(login);
        if (empId == null) return false;

        var cacheKey = $"RoleCache_IsApprover_{empId.Value}";
        if (_cache.TryGetValue(cacheKey, out bool cached)) return cached;

        try
        {
            using var ctx = _dbContextFactory.CreateDbContext();
            var result =
                ctx.Employees.AsNoTracking().Any(e => e.ReportToEmployeeId == empId.Value) ||
                ctx.Set<KCCMaterialFlow.Domain.Entities.WorkflowApprobateurSpecial>()
                    .AsNoTracking().Any(w => w.EmployeeId == empId.Value && w.EstActif);
            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }
        catch { return false; }
    }

    /// <summary>Invalide les entrées de cache pour cet utilisateur (login, matricule ou code employé).</summary>
    public void InvalidateUser(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        var k = key.ToUpperInvariant();
        _cache.Remove($"RoleCache_AppUser_{k}");
        _cache.Remove($"RoleCache_EmpId_{k}");
        // Invalider aussi par SAM si le login contient un domaine
        var sam = ExtractMatricule(key);
        if (sam != null && sam != k)
        {
            _cache.Remove($"RoleCache_AppUser_{sam}");
            _cache.Remove($"RoleCache_EmpId_{sam}");
        }
    }

    // ── Helpers privés ──────────────────────────────────────────────────

    private NiveauAdmin GetNiveauAdmin(string? login)
    {
        return GetCachedAppUser(login)?.NiveauAdmin ?? NiveauAdmin.Aucun;
    }

    private int? GetEmployeeId(string? login)
    {
        return GetCachedAppUser(login)?.EmployeeId;
    }

    private KCCMaterialFlow.Domain.Entities.AppUser? GetCachedAppUser(string? login)
    {
        if (string.IsNullOrWhiteSpace(login)) return null;

        var cacheKey = $"RoleCache_AppUser_{login.ToUpperInvariant()}";
        if (_cache.TryGetValue(cacheKey, out KCCMaterialFlow.Domain.Entities.AppUser? cached))
            return cached;

        try
        {
            using var ctx = _dbContextFactory.CreateDbContext();
            var loginUpper = login.ToUpperInvariant();
            var appUser = ctx.AppUsers.AsNoTracking()
                .FirstOrDefault(u => u.EstActif && u.Login.ToUpper() == loginUpper);

            // Fallback SAM : DOMAIN\K26561
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
        catch { return null; }
    }
}
