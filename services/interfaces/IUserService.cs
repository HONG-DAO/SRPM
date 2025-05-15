using SRPM.Models;
using System.Threading.Tasks;

namespace SRPM.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> FindOrCreateUserAsync(User user);
        Task<User?> GetByIdAsync(string id);
        Task UpdateUserProfileAsync(string id, string fullName, string avatarUrl, string bio, string socialLinks);
        Task UpdateUserRoleAsync(string id, string role);
        Task<List<User>> GetAllAsync(string? role = null);
    }
}
