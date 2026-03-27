using AppPlusPlus.Domain.Entities.Administration;

namespace AppPlusPlus.Application.Services.Administration;

public interface IUserAdminService
{
    Task<User?> GetUserByLoginAsync(string login);
    Task<User?> GetUserWithRoleAsync(string login);
    Task<bool> UserExistsAsync(string login);
    Task<List<Role>> GetActiveRolesAsync();
    Task<List<Activity>> GetActiveActivitiesWithFonctionAsync();
    Task<List<int>> GetUserGrantedActivityIdsAsync(string login);
    Task<HashSet<int>> GetRoleDefaultActivityIdsAsync(int? roleId);
    Task CreateUserAsync(User user, HashSet<int> localisationIds);
    Task UpdateUserAsync(User user, HashSet<int> localisationIds);
    Task SaveUserAccessAsync(string login, int? roleId, List<int> grantedActivityIds);
    Task UpdateProfileAsync(string login, string name, string email);
    Task<bool> ChangePasswordAsync(string login, string currentPassword, string newPassword);
    Task<List<UserActivity>> GetUserActivitiesWithDetailsAsync(string login);
    Task<List<User>> GetUsersByLocalisationAsync(int localisationId);
}
