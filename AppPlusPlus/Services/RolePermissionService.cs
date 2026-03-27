using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Infrastructure.Persistence;

namespace AppPlusPlus.Services;

/// <summary>
/// Service for initializing default permissions for standard roles
/// </summary>
public class RolePermissionService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public RolePermissionService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Initialize default permissions for all standard roles that have no permissions yet
    /// </summary>
    public async Task InitializeDefaultPermissionsAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var roles = await ctx.Roles.Where(r => r.IsActive).ToListAsync();
        var fonctions = await ctx.Fonctions.ToListAsync();

        foreach (var role in roles)
        {
            // Check if role already has permissions
            var hasPermissions = await ctx.Permissions.AnyAsync(p => p.RoleId == role.RoleId);
            if (hasPermissions) continue;

            // Get default permissions based on role name
            var defaultPerms = GetDefaultPermissionsForRole(role.DescriptionRole, fonctions);

            if (defaultPerms.Any())
            {
                foreach (var perm in defaultPerms)
                {
                    perm.RoleId = role.RoleId;
                }
                await ctx.Permissions.AddRangeAsync(defaultPerms);
            }
        }

        await ctx.SaveChangesAsync();
    }

    /// <summary>
    /// Set default permissions for a specific role by ID
    /// </summary>
    public async Task SetDefaultPermissionsForRoleAsync(int roleId, bool overwrite = false)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var role = await ctx.Roles.FindAsync(roleId);
        if (role == null) return;

        // Remove existing permissions if overwriting
        if (overwrite)
        {
            var existing = await ctx.Permissions.Where(p => p.RoleId == roleId).ToListAsync();
            ctx.Permissions.RemoveRange(existing);
            await ctx.SaveChangesAsync();
        }
        else
        {
            // Check if role already has permissions
            var hasPermissions = await ctx.Permissions.AnyAsync(p => p.RoleId == roleId);
            if (hasPermissions) return;
        }

        var fonctions = await ctx.Fonctions.ToListAsync();
        var defaultPerms = GetDefaultPermissionsForRole(role.DescriptionRole, fonctions);

        if (defaultPerms.Any())
        {
            foreach (var perm in defaultPerms)
            {
                perm.RoleId = roleId;
            }
            await ctx.Permissions.AddRangeAsync(defaultPerms);
            await ctx.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Get default permissions based on role name
    /// </summary>
    private List<Permission> GetDefaultPermissionsForRole(string? roleName, List<Fonction> fonctions)
    {
        var permissions = new List<Permission>();
        var normalizedRole = NormalizeRoleName(roleName);

        foreach (var fonction in fonctions)
        {
            var perm = GetPermissionForRoleAndFonction(normalizedRole, fonction);
            if (perm != null)
            {
                permissions.Add(perm);
            }
        }

        return permissions;
    }

    private string NormalizeRoleName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        return name.ToLowerInvariant()
            .Replace("é", "e")
            .Replace("è", "e")
            .Replace("ê", "e")
            .Replace("à", "a")
            .Replace("â", "a")
            .Replace("î", "i")
            .Replace("ô", "o")
            .Replace("û", "u")
            .Trim();
    }

    private Permission? GetPermissionForRoleAndFonction(string normalizedRole, Fonction fonction)
    {
        var fonctionName = NormalizeRoleName(fonction.DescriptionFonction);

        // Define permission matrix based on role
        return normalizedRole switch
        {
            "administrateur" or "admin" => new Permission
            {
                FonctionId = fonction.IdFonction,
                CanRead = true,
                CanWrite = true,
                CanDelete = true
            },

            "gerant" => GetGerantPermission(fonction, fonctionName),
            "caissier" or "caissiere" => GetCaissierPermission(fonction, fonctionName),
            "magasinier" => GetMagasinierPermission(fonction, fonctionName),
            "vendeur" or "vendeuse" => GetVendeurPermission(fonction, fonctionName),

            _ => null
        };
    }

    private Permission? GetGerantPermission(Fonction fonction, string fonctionName)
    {
        // Gérant has access to most features with write access
        var salesRelated = new[] { "vente", "facture", "caisse", "client", "ticket" };
        var stockRelated = new[] { "stock", "article", "appro", "inventaire", "catalogue" };
        var managementRelated = new[] { "rapport", "dashboard", "tableau", "statistique" };

        bool canRead = true;
        bool canWrite = salesRelated.Any(s => fonctionName.Contains(s)) ||
                        stockRelated.Any(s => fonctionName.Contains(s)) ||
                        managementRelated.Any(s => fonctionName.Contains(s));
        bool canDelete = salesRelated.Any(s => fonctionName.Contains(s));

        if (!canRead && !canWrite && !canDelete) return null;

        return new Permission
        {
            FonctionId = fonction.IdFonction,
            CanRead = canRead,
            CanWrite = canWrite,
            CanDelete = canDelete
        };
    }

    private Permission? GetCaissierPermission(Fonction fonction, string fonctionName)
    {
        // Caissier focuses on sales and cash operations
        var salesRelated = new[] { "vente", "facture", "caisse", "ticket", "paiement", "encaissement" };
        var viewOnly = new[] { "client", "article", "stock", "catalogue" };

        bool isSales = salesRelated.Any(s => fonctionName.Contains(s));
        bool isViewOnly = viewOnly.Any(s => fonctionName.Contains(s));

        if (!isSales && !isViewOnly) return null;

        return new Permission
        {
            FonctionId = fonction.IdFonction,
            CanRead = true,
            CanWrite = isSales,
            CanDelete = false
        };
    }

    private Permission? GetMagasinierPermission(Fonction fonction, string fonctionName)
    {
        // Magasinier focuses on stock and inventory
        var stockRelated = new[] { "stock", "article", "appro", "inventaire", "catalogue", "transformation", "localisation" };
        var viewOnly = new[] { "vente", "commande" };

        bool isStock = stockRelated.Any(s => fonctionName.Contains(s));
        bool isViewOnly = viewOnly.Any(s => fonctionName.Contains(s));

        if (!isStock && !isViewOnly) return null;

        return new Permission
        {
            FonctionId = fonction.IdFonction,
            CanRead = true,
            CanWrite = isStock,
            CanDelete = isStock
        };
    }

    private Permission? GetVendeurPermission(Fonction fonction, string fonctionName)
    {
        // Vendeur has limited access to sales
        var salesRelated = new[] { "vente", "client", "ticket" };
        var viewOnly = new[] { "article", "stock", "catalogue" };

        bool isSales = salesRelated.Any(s => fonctionName.Contains(s));
        bool isViewOnly = viewOnly.Any(s => fonctionName.Contains(s));

        if (!isSales && !isViewOnly) return null;

        return new Permission
        {
            FonctionId = fonction.IdFonction,
            CanRead = true,
            CanWrite = isSales,
            CanDelete = false
        };
    }
}
