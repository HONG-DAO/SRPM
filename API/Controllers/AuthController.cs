using Microsoft.AspNetCore.Mvc;
using SRPM.API.Models;
using SRPM.API.Services;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
        [HttpPost("verify-email")]
        public async Task<ActionResult<AuthResponse>> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.VerificationCode))
            {
                return BadRequest(new AuthResponse { Success = false, Message = "Email and verification code are required." });
            }

            var response = await _authService.VerifyEmailAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost("send-otp-for-register")]
        public async Task<ActionResult<AuthResponse>> SendOtpForRegister([FromBody] SendOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new AuthResponse { Success = false, Message = "Email is required." });
            }

            var response = await _authService.SendOtpForRegisterAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
