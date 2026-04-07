using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des rôles avec mise en cache.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class RoleService : IRoleService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RoleService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string CacheKeyAll = "Roles_All";
    private const string CacheKeyPrefix = "Role_";

    public RoleService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<RoleService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAll, out IReadOnlyList<Role>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var roles = await context.Set<Role>()
            .AsNoTracking()
            .Where(r => r.EstActif)
            .OrderByDescending(r => r.NiveauPriorite)
            .ThenBy(r => r.NomRole)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAll, (IReadOnlyList<Role>)roles, CacheDuration);
        return roles;
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        
        if (_cache.TryGetValue(cacheKey, out Role? cached))
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var role = await context.Set<Role>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role != null)
            _cache.Set(cacheKey, role, CacheDuration);

        return role;
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<Role>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.CodeRole.ToUpper() == code.ToUpper() && r.EstActif, cancellationToken);
    }

    public async Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        role.DateCreation = DateTime.Now;
        role.CodeRole = role.CodeRole.ToUpperInvariant();

        context.Set<Role>().Add(role);
        await context.SaveChangesAsync(cancellationToken);

        InvalidateCache();
        _logger.LogInformation("Rôle créé: {Code} - {Nom}", role.CodeRole, role.NomRole);

        return role;
    }

    public async Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == role.Id, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Rôle {role.Id} non trouvé");

        if (existing.EstSysteme)
            throw new InvalidOperationException("Les rôles système ne peuvent pas être modifiés");

        existing.CodeRole = role.CodeRole.ToUpperInvariant();
        existing.NomRole = role.NomRole;
        existing.Description = role.Description;
        existing.NiveauPriorite = role.NiveauPriorite;
        existing.EstActif = role.EstActif;
        existing.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Rôle modifié: {Code}", existing.CodeRole);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var role = await context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return false;

        if (role.EstSysteme)
            throw new InvalidOperationException("Les rôles système ne peuvent pas être supprimés");

        role.EstActif = false;
        role.DateModification = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);
        InvalidateCache();

        _logger.LogInformation("Rôle supprimé: {Code}", role.CodeRole);
        return true;
    }

    public async Task<IReadOnlyList<Role>> GetRolesForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<UtilisateurRole>()
            .AsNoTracking()
            .Where(ur => ur.IdUtilisateur == userId)
            .Include(ur => ur.Role)
            .Where(ur => ur.Role != null && ur.Role.EstActif)
            .Select(ur => ur.Role!)
            .OrderByDescending(r => r.NiveauPriorite)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AssignRoleToUserAsync(int userId, int roleId, string assignedByLogin, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var exists = await context.Set<UtilisateurRole>()
            .AnyAsync(ur => ur.IdUtilisateur == userId && ur.IdRole == roleId, cancellationToken);

        if (exists)
            return false;

        var userRole = new UtilisateurRole
        {
            IdUtilisateur = userId,
            IdRole = roleId,
            DateAttribution = DateTime.Now,
            AttribueParLogin = assignedByLogin
        };

        context.Set<UtilisateurRole>().Add(userRole);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rôle {RoleId} attribué à l'utilisateur {UserId} par {AssignedBy}", roleId, userId, assignedByLogin);
        return true;
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var userRole = await context.Set<UtilisateurRole>()
            .FirstOrDefaultAsync(ur => ur.IdUtilisateur == userId && ur.IdRole == roleId, cancellationToken);

        if (userRole == null)
            return false;

        context.Set<UtilisateurRole>().Remove(userRole);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rôle {RoleId} retiré de l'utilisateur {UserId}", roleId, userId);
        return true;
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Set<Role>()
            .Where(r => r.CodeRole.ToUpper() == code.ToUpper());

        if (excludeId.HasValue)
            query = query.Where(r => r.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    private void InvalidateCache()
    {
        _cache.Remove(CacheKeyAll);
    }
}
