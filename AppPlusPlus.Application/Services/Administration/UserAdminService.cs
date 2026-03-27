using AppPlusPlus.Domain.Entities.Administration;
using AppPlusPlus.Application.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Administration;

public class UserAdminService : IUserAdminService
{
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;

    public UserAdminService(IUserRepository userRepo, IRoleRepository roleRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _userRepo.GetByLoginAsync(login);
    }

    public async Task<User?> GetUserWithRoleAsync(string login)
    {
        return await _userRepo.GetWithRoleAsync(login);
    }

    public async Task<bool> UserExistsAsync(string login)
    {
        var user = await _userRepo.GetByLoginAsync(login);
        return user != null;
    }

    public async Task<List<Role>> GetActiveRolesAsync()
    {
        return await _roleRepo.GetActiveRolesAsync();
    }

    public async Task<List<Activity>> GetActiveActivitiesWithFonctionAsync()
    {
        var all = await _roleRepo.GetAllActivitiesAsync();
        return all.Where(a => a.IsActive).ToList();
    }

    public async Task<List<int>> GetUserGrantedActivityIdsAsync(string login)
    {
        var activities = await _userRepo.GetUserActivitiesAsync(login);
        return activities
            .Where(ua => ua.IsGranted)
            .Select(ua => ua.ActivityId)
            .ToList();
    }

    /// <summary>
    /// Maps role permissions to activity IDs based on suffix patterns:
    /// CanRead  -> activities whose Code ends with "_read"
    /// CanWrite -> activities whose Code ends with "_create" or "_update"
    /// CanDelete -> activities whose Code ends with "_delete"
    /// Matching is done by FonctionId (permission and activity share the same Fonction).
    /// </summary>
    public async Task<HashSet<int>> GetRoleDefaultActivityIdsAsync(int? roleId)
    {
        if (roleId is null)
            return new HashSet<int>();

        var permissions = await _roleRepo.GetPermissionsByRoleAsync(roleId.Value);
        var allActivities = await _roleRepo.GetAllActivitiesAsync();

        var result = new HashSet<int>();

        foreach (var perm in permissions)
        {
            var fonctionActivities = allActivities
                .Where(a => a.FonctionId == perm.FonctionId && a.IsActive)
                .ToList();

            foreach (var act in fonctionActivities)
            {
                var code = act.Code;

                if (perm.CanRead && code.EndsWith("_read", StringComparison.OrdinalIgnoreCase))
                    result.Add(act.ActivityId);

                if (perm.CanWrite &&
                    (code.EndsWith("_create", StringComparison.OrdinalIgnoreCase) ||
                     code.EndsWith("_update", StringComparison.OrdinalIgnoreCase)))
                    result.Add(act.ActivityId);

                if (perm.CanDelete && code.EndsWith("_delete", StringComparison.OrdinalIgnoreCase))
                    result.Add(act.ActivityId);
            }
        }

        return result;
    }

    public async Task CreateUserAsync(User user, HashSet<int> localisationIds)
    {
        await _userRepo.AddAsync(user);

        if (localisationIds.Count > 0)
        {
            await _userRepo.UpdateUserLocalisationsAsync(user.Login, localisationIds);
        }
    }

    public async Task UpdateUserAsync(User user, HashSet<int> localisationIds)
    {
        await _userRepo.UpdateAsync(user);
        await _userRepo.UpdateUserLocalisationsAsync(user.Login, localisationIds);
    }

    /// <summary>
    /// Updates the user's RoleId and replaces all UserActivity records.
    /// </summary>
    public async Task SaveUserAccessAsync(string login, int? roleId, List<int> grantedActivityIds)
    {
        var user = await _userRepo.GetByLoginAsync(login);
        if (user is null) return;

        user.RoleId = roleId;
        await _userRepo.UpdateAsync(user);

        await _userRepo.ReplaceUserActivitiesAsync(login, grantedActivityIds);
    }

    public async Task UpdateProfileAsync(string login, string name, string email)
    {
        var user = await _userRepo.GetByLoginAsync(login);
        if (user is null) return;

        user.Name = name;
        user.Email = email;
        await _userRepo.UpdateAsync(user);
    }

    public async Task<bool> ChangePasswordAsync(string login, string currentPassword, string newPassword)
    {
        var user = await _userRepo.GetByLoginAsync(login);
        if (user is null) return false;

        // Verify current password matches
        if (user.Password != currentPassword)
            return false;

        user.Password = newPassword;
        await _userRepo.UpdateAsync(user);
        return true;
    }

    public async Task<List<UserActivity>> GetUserActivitiesWithDetailsAsync(string login)
    {
        return await _userRepo.GetUserActivitiesAsync(login);
    }

    public async Task<List<User>> GetUsersByLocalisationAsync(int localisationId)
    {
        return await _userRepo.GetByLocalisationAsync(localisationId);
    }
}
