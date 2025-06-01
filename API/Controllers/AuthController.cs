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
        // private readonly IConfiguration _configuration;
        public AuthController(IAuthService authService, IGoogleAuthService googleAuthService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            // _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new AuthResponse { Success = false, Message = "Email and password are required." });

            var response = await _authService.LoginAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
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
        [HttpGet("google/signup")]
        public IActionResult InitiateGoogleSignUp()
        {
            try
            {
                var redirectUrl = _googleAuthService.GetGoogleAuthUrl("signup");
                return Ok(new { redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                // return BadRequest(new { error = "Failed to initiate Google sign up", message = ex.Message });
                 // Log lỗi (nếu bạn dùng ILogger thì log ở đây)
                Console.WriteLine("Exception in GoogleSignup: " + ex.ToString());

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("google/signin")]
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
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest(new AuthResponse { Success = false, Message = "Authorization code is required." });

                var response = await _googleAuthService.HandleGoogleCallbackAsync(code, "signup");
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Failed to process Google sign up callback", 
                    Error = ex.Message 
                });
            }
        }

        [HttpGet("google/signin/callback")]
        public async Task<IActionResult> HandleGoogleSignInCallback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Authorization code is required.");

            var response = await _googleAuthService.HandleGoogleCallbackAsync(code, "signin");
            if (!response.Success)
                return BadRequest(response);

            // Giả sử response chứa token
            var token = response.AccessToken;

            // Redirect về frontend, gửi token dưới dạng query param hoặc fragment
            var redirectUrl = $"http://aienthusiasm.vn:8080//thanhviennghiencuu?token={token}";

            return Redirect(redirectUrl);
        }

    }
}