using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM.API.Models;
using SRPM.API.Services;
using System.Security.Claims;

namespace SRPM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("role/{roleName}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetByRole(string roleName)
        {
            var users = await _userService.GetByRoleAsync(roleName);
            return Ok(users);
        }

        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _userService.UpdateProfileAsync(userId, request);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpPost("{userId}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddRole(int userId, [FromBody] AddRoleRequest request)
        {
            var success = await _userService.AddRoleAsync(userId, request.RoleName);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{userId}/roles/{roleName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(int userId, string roleName)
        {
            var success = await _userService.RemoveRoleAsync(userId, roleName);
            if (!success)
                return BadRequest();

            return NoContent();
        }
    }
}
