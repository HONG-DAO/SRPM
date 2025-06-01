using SRPM.API.Models;

namespace SRPM.API.Services
{
    public interface IGoogleAuthService
    {
        string GetGoogleAuthUrl(string action);
        Task<AuthResponse> HandleGoogleCallbackAsync(string code, string action);
    }
}