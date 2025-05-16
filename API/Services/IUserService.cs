using SRPM.API.Models;

namespace SRPM.API.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<IEnumerable<UserDto>> GetByRoleAsync(string roleName);
        Task<bool> UpdateProfileAsync(int id, UpdateProfileRequest request);
        Task<bool> AddRoleAsync(int userId, string roleName);
        Task<bool> RemoveRoleAsync(int userId, string roleName);
    }
}
