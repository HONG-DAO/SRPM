using Microsoft.AspNetCore.Mvc;
using SRPM.Services;
using SRPM.Dto;

namespace SRPM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Cập nhật hồ sơ người dùng
        /// </summary>
        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UpdateProfileDto dto)
        {
            await _userService.UpdateProfileAsync(id, dto.FullName, dto.AvatarUrl, dto.Bio, dto.SocialLinks);
            return Ok(new { message = "Đã cập nhật hồ sơ người dùng." });
        }

        /// <summary>
        /// Lấy tất cả người dùng (có thể lọc theo vai trò)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? role)
        {
            var users = await _userService.GetAllAsync(role);
            return Ok(users);
        }

        /// <summary>
        /// Cập nhật vai trò người dùng
        /// </summary>
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateUserRoleDto dto)
        {
            try
            {
                await _userService.UpdateRoleAsync(id, dto.Role);
                return Ok(new { message = "Vai trò người dùng đã được cập nhật." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
