using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using SRPM.Models;
using SRPM.Services;
using System.Threading.Tasks;

namespace SRPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Xác thực Google và đăng nhập/đăng ký
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            }
            catch
            {
                return Unauthorized("Token không hợp lệ");
            }

            // Chỉ cho phép email từ tổ chức
            if (!payload.Email.EndsWith("@fe.edu.vn"))
            {
                return Forbid("Chỉ cho phép tài khoản @fe.edu.vn");
            }

            // Tạo hoặc lấy user từ DB
            var user = await _userService.FindOrCreateUserAsync(new User
            {
                Email = payload.Email,
                FullName = payload.Name,
                AvatarUrl = payload.Picture,
                Role = "Researcher" // Mặc định
            });

            var jwt = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token = jwt,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.Role,
                    user.AvatarUrl
                }
            });
        }
    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }
}
