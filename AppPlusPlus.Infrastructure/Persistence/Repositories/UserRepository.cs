using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Domain.Entities.Administration;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Infrastructure.Persistence.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(IDbContextFactory<AppDbContext> dbFactory) : base(dbFactory) { }

    public async Task<User?> GetByLoginAsync(string login)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Users.FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<User?> GetWithLocalisationsAsync(string login)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Users
            .Include(u => u.UserLocalisations).ThenInclude(ul => ul.Localisation)
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<User?> GetWithActivitiesAsync(string login)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Users
            .Include(u => u.UserActivities).ThenInclude(ua => ua.Activity).ThenInclude(a => a!.Fonction)
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<List<User>> GetByRoleAsync(int roleId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Users.Where(u => u.RoleId == roleId).ToListAsync();
    }

    public async Task<List<User>> GetActiveUsersAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Users.Where(u => u.Activated == true).ToListAsync();
    }

    public async Task<List<UserLocalisation>> GetUserLocalisationsAsync(string login)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.UserLocalisations
            .Where(ul => ul.UserId == login)
            .Include(ul => ul.Localisation)
            .ToListAsync();
    }

    public async Task<List<UserActivity>> GetUserActivitiesAsync(string login)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.UserActivities
            .Where(ua => ua.UserLogin == login)
            .Include(ua => ua.Activity).ThenInclude(a => a!.Fonction)
            .ToListAsync();
    }

    public async Task SetUserLocalisationsAsync(string login, List<int> localisationIds)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var existing = await ctx.UserLocalisations.Where(ul => ul.UserId == login).ToListAsync();
        ctx.UserLocalisations.RemoveRange(existing);
        foreach (var locId in localisationIds)
        {
            ctx.UserLocalisations.Add(new UserLocalisation { UserId = login, LocalisationId = locId });
        }
        await ctx.SaveChangesAsync();
    }
}
