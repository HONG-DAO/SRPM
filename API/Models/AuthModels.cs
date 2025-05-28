namespace SRPM.API.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class GoogleLoginRequest
    {
        public string GoogleToken { get; set; }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public UserDto? User { get; set; }
        public string? Message { get; set; }
    }
    public class SendOtpRequest
    {
        public string Email { get; set; } = string.Empty;
    }
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

}
