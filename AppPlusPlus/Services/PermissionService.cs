using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Data;

namespace AppPlusPlus.Services;

public static class AppFunctions
{
    public const string Dashboard = "dashboard";
    public const string Vente = "vente";
    public const string Facturation = "facturation";
    public const string Stock = "stock";
    public const string Approvisionnement = "approvisionnement";
    public const string CommandesClients = "commandes clients";
    public const string CommandesInternes = "commandes internes";
    public const string Livraison = "livraison";
    public const string Rapports = "rapports";
    public const string Administration = "administration";
    public const string Parametres = "parametres";

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace("_", " ")
            .Replace("-", " ");
    }
}

public sealed class PermissionSnapshot
{
    public static PermissionSnapshot Empty { get; } = new([], [], []);

    private readonly HashSet<string> _read;
    private readonly HashSet<string> _write;
    private readonly HashSet<string> _delete;

    public PermissionSnapshot(IEnumerable<string> read, IEnumerable<string> write, IEnumerable<string> delete)
    {
        _read = new HashSet<string>(read.Select(AppFunctions.Normalize));
        _write = new HashSet<string>(write.Select(AppFunctions.Normalize));
        _delete = new HashSet<string>(delete.Select(AppFunctions.Normalize));
    }

    public bool CanRead(string functionName) => _read.Contains(AppFunctions.Normalize(functionName));
    public bool CanWrite(string functionName) => _write.Contains(AppFunctions.Normalize(functionName));
    public bool CanDelete(string functionName) => _delete.Contains(AppFunctions.Normalize(functionName));
}

public class PermissionService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public PermissionService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<PermissionSnapshot> GetPermissionsAsync(string? login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return PermissionSnapshot.Empty;

        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var userActivities = await ctx.UserActivities
            .Include(ua => ua.Activity)
                .ThenInclude(a => a!.Fonction)
            .Where(ua => ua.UserLogin == login && ua.IsGranted)
            .ToListAsync();

        if (userActivities.Any())
        {
            var read = new HashSet<string>();
            var write = new HashSet<string>();
            var delete = new HashSet<string>();

            foreach (var userActivity in userActivities)
            {
                var activity = userActivity.Activity;
                var fonction = activity?.Fonction?.DescriptionFonction;
                if (string.IsNullOrWhiteSpace(fonction) || string.IsNullOrWhiteSpace(activity?.Code))
                    continue;

                var code = activity.Code.Trim().ToLowerInvariant();

                if (code.EndsWith("_read"))
                {
                    read.Add(fonction);
                }
                else if (code.EndsWith("_create") || code.EndsWith("_update"))
                {
                    read.Add(fonction);
                    write.Add(fonction);
                }
                else if (code.EndsWith("_delete"))
                {
                    read.Add(fonction);
                    write.Add(fonction);
                    delete.Add(fonction);
                }
            }

            return new PermissionSnapshot(read, write, delete);
        }

        var roleId = await ctx.Users
            .Where(u => u.Login == login && u.Activated == true)
            .Select(u => u.RoleId)
            .FirstOrDefaultAsync();

        if (!roleId.HasValue)
            return PermissionSnapshot.Empty;

        var permissions = await ctx.Permissions
            .Include(p => p.Fonction)
            .Where(p => p.RoleId == roleId.Value)
            .ToListAsync();

        return new PermissionSnapshot(
            permissions.Where(p => p.CanRead).Select(p => p.Fonction?.DescriptionFonction ?? string.Empty),
            permissions.Where(p => p.CanWrite).Select(p => p.Fonction?.DescriptionFonction ?? string.Empty),
            permissions.Where(p => p.CanDelete).Select(p => p.Fonction?.DescriptionFonction ?? string.Empty));
    }
}
