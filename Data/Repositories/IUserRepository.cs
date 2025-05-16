using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByRoleAsync(string roleName);
        Task<bool> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        Task<bool> AddUserRoleAsync(int userId, int roleId);
        Task<bool> RemoveUserRoleAsync(int userId, int roleId);
    }
}
