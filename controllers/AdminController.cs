using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM.Models;
using SRPM.Services;
using System.Threading.Tasks;

namespace SRPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStatsService _statsService;
        private readonly ISettingsService _settingsService;

        public AdminController(IUserService userService, IStatsService statsService, ISettingsService settingsService)
        {
            _userService = userService;
            _statsService = statsService;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Cập nhật vai trò của người dùng (phân quyền)
        /// </summary>
        [HttpPut("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentRequest request)
        {
            var result = await _userService.AssignRoleAsync(request.UserId, request.NewRole);
            if (!result)
                return BadRequest("Không thể cập nhật vai trò.");
            return Ok("Cập nhật vai trò thành công.");
        }

        /// <summary>
        /// Lấy thống kê tổng quan hệ thống
        /// </summary>
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            var stats = await _statsService.GetSystemStatisticsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Lấy cấu hình hệ thống hiện tại
        /// </summary>
        [HttpGet("settings")]
        public IActionResult GetSettings()
        {
            var settings = _settingsService.GetSettings();
            return Ok(settings);
        }

        /// <summary>
        /// Cập nhật cấu hình hệ thống
        /// </summary>
        [HttpPut("settings")]
        public IActionResult UpdateSettings([FromBody] SystemSettings newSettings)
        {
            _settingsService.UpdateSettings(newSettings);
            return Ok("Đã cập nhật cấu hình hệ thống.");
        }
    }

    public class RoleAssignmentRequest
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }
}
