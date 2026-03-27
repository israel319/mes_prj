using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Domain.Entities.Administration;
using AppPlusPlus.Application.Interfaces.Repositories;

namespace AppPlusPlus.Infrastructure.Persistence.Repositories;

public class RoleRepository : RepositoryBase<Role>, IRoleRepository
{
    public RoleRepository(IDbContextFactory<AppDbContext> dbFactory) : base(dbFactory) { }

    public async Task<Role?> GetWithPermissionsAsync(int roleId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Roles
            .Include(r => r.Permissions).ThenInclude(p => p.Fonction)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
    }

    public async Task<List<Role>> GetActiveRolesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Roles.Where(r => r.IsActive).ToListAsync();
    }

    public async Task<List<Permission>> GetPermissionsByRoleAsync(int roleId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Permissions
            .Where(p => p.RoleId == roleId)
            .Include(p => p.Fonction)
            .ToListAsync();
    }

    public async Task SetPermissionsAsync(int roleId, List<Permission> permissions)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var existing = await ctx.Permissions.Where(p => p.RoleId == roleId).ToListAsync();
        ctx.Permissions.RemoveRange(existing);
        foreach (var perm in permissions)
        {
            perm.RoleId = roleId;
            ctx.Permissions.Add(perm);
        }
        await ctx.SaveChangesAsync();
    }

    public async Task<List<Fonction>> GetAllFonctionsAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Fonctions.ToListAsync();
    }

    public async Task<List<Activity>> GetAllActivitiesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Activities.Include(a => a.Fonction).ToListAsync();
    }

    public async Task<List<Activity>> GetActivitiesByFonctionAsync(int fonctionId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Activities.Where(a => a.FonctionId == fonctionId).ToListAsync();
    }
}
