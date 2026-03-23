using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using KCCMaterialFlow.Module.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des permissions avec mise en cache.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PermissionService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string CacheKeyAll = "Permissions_All";
    private const string CacheKeyRolePrefix = "Permissions_Role_";

    public PermissionService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<PermissionService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAll, out IReadOnlyList<Permission>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var permissions = await context.Set<Permission>()
            .AsNoTracking()
            .Where(p => p.EstActif)
            .OrderBy(p => p.Categorie)
            .ThenBy(p => p.OrdreAffichage)
            .ThenBy(p => p.NomPermission)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAll, (IReadOnlyList<Permission>)permissions, CacheDuration);
        return permissions;
    }

    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Permission>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPermission == id, cancellationToken);
    }

    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Permission>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CodePermission == code && p.EstActif, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByCategorieAsync(string categorie, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Permission>()
            .AsNoTracking()
            .Where(p => p.Categorie == categorie && p.EstActif)
            .OrderBy(p => p.OrdreAffichage)
            .ThenBy(p => p.NomPermission)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsForRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyRolePrefix}{roleId}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Permission>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var permissions = await context.Set<RolePermission>()
            .AsNoTracking()
            .Where(rp => rp.IdRole == roleId)
            .Include(rp => rp.Permission)
            .Where(rp => rp.Permission != null && rp.Permission.EstActif)
            .Select(rp => rp.Permission!)
            .OrderBy(p => p.Categorie)
            .ThenBy(p => p.OrdreAffichage)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Permission>)permissions, CacheDuration);
        return permissions;
    }

    public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var exists = await context.Set<RolePermission>()
            .AnyAsync(rp => rp.IdRole == roleId && rp.IdPermission == permissionId, cancellationToken);

        if (exists)
            return false;

        context.Set<RolePermission>().Add(new RolePermission
        {
            IdRole = roleId,
            IdPermission = permissionId,
            DateAttribution = DateTime.Now
        });

        await context.SaveChangesAsync(cancellationToken);
        InvalidateRolePermissionsCache(roleId);
        _logger.LogInformation("Permission {PermId} attribuée au rôle {RoleId}", permissionId, roleId);
        return true;
    }

    public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var rp = await context.Set<RolePermission>()
            .FirstOrDefaultAsync(rp => rp.IdRole == roleId && rp.IdPermission == permissionId, cancellationToken);

        if (rp == null)
            return false;

        context.Set<RolePermission>().Remove(rp);
        await context.SaveChangesAsync(cancellationToken);
        InvalidateRolePermissionsCache(roleId);
        _logger.LogInformation("Permission {PermId} retirée du rôle {RoleId}", permissionId, roleId);
        return true;
    }

    public async Task UpdateRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Supprimer les permissions actuelles du rôle
        var existing = await context.Set<RolePermission>()
            .Where(rp => rp.IdRole == roleId)
            .ToListAsync(cancellationToken);

        context.Set<RolePermission>().RemoveRange(existing);

        // Ajouter les nouvelles permissions
        var newPermissions = permissionIds.Select(pid => new RolePermission
        {
            IdRole = roleId,
            IdPermission = pid,
            DateAttribution = DateTime.Now
        });

        context.Set<RolePermission>().AddRange(newPermissions);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateRolePermissionsCache(roleId);
        _logger.LogInformation("Permissions du rôle {RoleId} mises à jour ({Count} permissions)", roleId, permissionIds.Count());
    }

    public async Task<bool> RoleHasPermissionAsync(int roleId, string codePermission, CancellationToken cancellationToken = default)
    {
        var permissions = await GetPermissionsForRoleAsync(roleId, cancellationToken);
        return permissions.Any(p => p.CodePermission == codePermission || p.CodePermission == "ALL");
    }

    private void InvalidateRolePermissionsCache(int roleId)
    {
        _cache.Remove($"{CacheKeyRolePrefix}{roleId}");
    }
}
