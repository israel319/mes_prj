using AppPlusPlus.Domain.Common;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Administration;

public class PermissionResolver : IPermissionResolver
{
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;

    public PermissionResolver(IUserRepository userRepo, IRoleRepository roleRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
    }

    /// <inheritdoc />
    public async Task<PermissionSnapshot> GetPermissionsAsync(string? login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return PermissionSnapshot.Empty;

        // ---- Try user-level activity grants first ----
        var userActivities = await _userRepo.GetUserActivitiesAsync(login);

        if (userActivities.Any(ua => ua.IsGranted))
        {
            var read = new HashSet<string>();
            var write = new HashSet<string>();
            var delete = new HashSet<string>();

            foreach (var userActivity in userActivities.Where(ua => ua.IsGranted))
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

        // ---- Fall back to role-based permissions ----
        var user = await _userRepo.GetByLoginAsync(login);
        if (user?.RoleId == null || user.Activated != true)
            return PermissionSnapshot.Empty;

        var permissions = await _roleRepo.GetPermissionsByRoleAsync(user.RoleId.Value);

        return new PermissionSnapshot(
            permissions.Where(p => p.CanRead).Select(p => p.Fonction?.DescriptionFonction ?? string.Empty),
            permissions.Where(p => p.CanWrite).Select(p => p.Fonction?.DescriptionFonction ?? string.Empty),
            permissions.Where(p => p.CanDelete).Select(p => p.Fonction?.DescriptionFonction ?? string.Empty));
    }
}
