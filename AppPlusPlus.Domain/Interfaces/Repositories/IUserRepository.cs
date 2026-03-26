using AppPlusPlus.Domain.Entities.Administration;

namespace AppPlusPlus.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetWithLocalisationsAsync(string login);
    Task<User?> GetWithActivitiesAsync(string login);
    Task<List<User>> GetByRoleAsync(int roleId);
    Task<List<User>> GetActiveUsersAsync();
    Task<List<UserLocalisation>> GetUserLocalisationsAsync(string login);
    Task<List<UserActivity>> GetUserActivitiesAsync(string login);
    Task SetUserLocalisationsAsync(string login, List<int> localisationIds);
}
