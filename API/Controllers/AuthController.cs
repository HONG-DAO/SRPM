using Microsoft.AspNetCore.Mvc;
using SRPM.API.Models;
using SRPM.API.Services;
using System.Web;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IConfiguration _configuration;
        
        public AuthController(IAuthService authService, IGoogleAuthService googleAuthService, IConfiguration configuration)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Email and password are required." });

            var response = await _authService.LoginAsync(request);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message });

            return Ok(new { success = true, token = response.Token, user = response.User });
        }


        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new AuthResponse { Success = false, Message = "Email, name, and password are required." });

            var response = await _authService.RegisterAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("google-login")]
        public async Task<ActionResult<AuthResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.GoogleToken))
                return BadRequest(new AuthResponse { Success = false, Message = "Google token is required." });

            var response = await _authService.LoginWithGoogleAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Google OAuth2 Endpoints
        [HttpGet("google/signin")]
        public IActionResult InitiateGoogleSignUp()
        {
            try
            {
                var redirectUrl = _googleAuthService.GetGoogleAuthUrl("signup");
                return Ok(new { redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in GoogleSignup: " + ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("google/signup")]
        public IActionResult InitiateGoogleSignIn()
        {
            try
            {
                var redirectUrl = _googleAuthService.GetGoogleAuthUrl("signin");
                return Ok(new { redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to initiate Google sign in", message = ex.Message });
            }
        }

        [HttpGet("google/signup/callback")]
        public async Task<ActionResult<AuthResponse>> HandleGoogleSignUpCallback([FromQuery] string code, [FromQuery] string state)
        {
            // Lấy frontend URL một lần duy nhất
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:8081";
            
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest(new AuthResponse { Success = false, Message = "Authorization code is required." });

                var response = await _googleAuthService.HandleGoogleCallbackAsync(code, "signup");
                if (!response.Success)
                {
                    // Redirect về frontend với error
                    var errorUrl = $"{frontendUrl}/google-callback?error={Uri.EscapeDataString(response.Message)}";
                    return Redirect(errorUrl);
                }

                // Nếu đăng ký thành công, redirect về trang đăng nhập
                var redirectUrl = $"{frontendUrl}/thanhviennghiencuu?message={Uri.EscapeDataString("Registration successful! Please sign in.")}";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in HandleGoogleSignUpCallback: " + ex.ToString());
                
                // Redirect về frontend với error
                var errorUrl = $"{frontendUrl}/google-callback?error={Uri.EscapeDataString("Registration failed")}";
                return Redirect(errorUrl);
            }
        }

        [HttpGet("google/signin/callback")]
        public async Task<IActionResult> HandleGoogleSignInCallback([FromQuery] string code, [FromQuery] string state)
        {
            // Lấy frontend URL một lần duy nhất
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:8081";
            
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest("Authorization code is required.");

                var response = await _googleAuthService.HandleGoogleCallbackAsync(code, "signin");
                if (!response.Success)
                {
                    // Redirect về frontend với error
                    var errorUrl = $"{frontendUrl}/google-callback?error={Uri.EscapeDataString(response.Message)}";
                    return Redirect(errorUrl);
                }

                // Giả sử response chứa token
                var token = response.AccessToken;

                // Redirect về frontend với token
                var redirectUrl = $"{frontendUrl}/google-callback?token={Uri.EscapeDataString(token)}&success=true";

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in HandleGoogleSignInCallback: " + ex.ToString());
                
                // Redirect về frontend với error
                var errorUrl = $"{frontendUrl}/google-callback?error={Uri.EscapeDataString("Authentication failed")}";
                return Redirect(errorUrl);
            }
        }
    }
}