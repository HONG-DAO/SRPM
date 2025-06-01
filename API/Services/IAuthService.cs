using SRPM.API.Models;
using SRPM.Data.Entities;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<string> GenerateJwtTokenAsync(User user);
        Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request);
        Task<UserDto?> GetUserByEmailAsync(string email);
    }
}
