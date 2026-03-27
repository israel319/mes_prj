using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Infrastructure.Persistence;
using AppPlusPlus.Domain.Common;

namespace AppPlusPlus.Services;

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
