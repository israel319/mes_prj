using AppPlusPlus.Domain.Entities.Administration;
using AppPlusPlus.Application.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Administration;

/// <summary>
/// Service for initializing default permissions for standard roles.
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepo;

    public RoleService(IRoleRepository roleRepo)
    {
        _roleRepo = roleRepo;
    }

    /// <inheritdoc />
    public async Task InitializeDefaultPermissionsAsync()
    {
        var roles = await _roleRepo.GetActiveRolesAsync();
        var fonctions = await _roleRepo.GetAllFonctionsAsync();

        foreach (var role in roles)
        {
            var existing = await _roleRepo.GetPermissionsByRoleAsync(role.RoleId);
            if (existing.Any()) continue;

            var defaultPerms = GetDefaultPermissionsForRole(role.DescriptionRole, fonctions);
            if (defaultPerms.Any())
            {
                foreach (var perm in defaultPerms)
                    perm.RoleId = role.RoleId;

                await _roleRepo.SetPermissionsAsync(role.RoleId, defaultPerms);
            }
        }
    }

    /// <inheritdoc />
    public async Task SetDefaultPermissionsForRoleAsync(int roleId, bool overwrite = false)
    {
        var role = await _roleRepo.GetByIdAsync(roleId);
        if (role == null) return;

        if (overwrite)
        {
            // SetPermissionsAsync replaces all permissions for the role
            var fonctions = await _roleRepo.GetAllFonctionsAsync();
            var defaultPerms = GetDefaultPermissionsForRole(role.DescriptionRole, fonctions);

            foreach (var perm in defaultPerms)
                perm.RoleId = roleId;

            await _roleRepo.SetPermissionsAsync(roleId, defaultPerms);
        }
        else
        {
            var existing = await _roleRepo.GetPermissionsByRoleAsync(roleId);
            if (existing.Any()) return;

            var fonctions = await _roleRepo.GetAllFonctionsAsync();
            var defaultPerms = GetDefaultPermissionsForRole(role.DescriptionRole, fonctions);

            foreach (var perm in defaultPerms)
                perm.RoleId = roleId;

            await _roleRepo.SetPermissionsAsync(roleId, defaultPerms);
        }
    }

    // ----------------------------------------------------------------
    //  Permission matrix helpers (migrated from RolePermissionService)
    // ----------------------------------------------------------------

    private List<Permission> GetDefaultPermissionsForRole(string? roleName, List<Fonction> fonctions)
    {
        var permissions = new List<Permission>();
        var normalizedRole = NormalizeRoleName(roleName);

        foreach (var fonction in fonctions)
        {
            var perm = GetPermissionForRoleAndFonction(normalizedRole, fonction);
            if (perm != null)
                permissions.Add(perm);
        }

        return permissions;
    }

    private static string NormalizeRoleName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        return name.ToLowerInvariant()
            .Replace("\u00e9", "e")   // e with acute
            .Replace("\u00e8", "e")   // e with grave
            .Replace("\u00ea", "e")   // e with circumflex
            .Replace("\u00e0", "a")   // a with grave
            .Replace("\u00e2", "a")   // a with circumflex
            .Replace("\u00ee", "i")   // i with circumflex
            .Replace("\u00f4", "o")   // o with circumflex
            .Replace("\u00fb", "u")   // u with circumflex
            .Trim();
    }

    private static Permission? GetPermissionForRoleAndFonction(string normalizedRole, Fonction fonction)
    {
        var fonctionName = NormalizeRoleName(fonction.DescriptionFonction);

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

    private static Permission? GetGerantPermission(Fonction fonction, string fonctionName)
    {
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

    private static Permission? GetCaissierPermission(Fonction fonction, string fonctionName)
    {
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

    private static Permission? GetMagasinierPermission(Fonction fonction, string fonctionName)
    {
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

    private static Permission? GetVendeurPermission(Fonction fonction, string fonctionName)
    {
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
