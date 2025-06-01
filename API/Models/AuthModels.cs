namespace SRPM.API.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? BackgroundUrl { get; set; } // Thêm property này
        public List<string> SocialLinks { get; set; } = new List<string>(); // Thêm property này
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsGoogleUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }

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
        public string? AccessToken { get; set; }
        public UserDto? User { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }

}
