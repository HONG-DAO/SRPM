using Microsoft.AspNetCore.Mvc;
using SRPM.Services;
using SRPM.Models;
using SRPM.Dto;

namespace SRPM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Tạo đề tài mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                OwnerId = dto.OwnerId,
            };

            var created = await _projectService.CreateProjectAsync(project);
            return Ok(new { message = "Đã tạo đề tài nghiên cứu.", projectId = created.Id });
        }

        /// <summary>
        /// Lấy danh sách tất cả đề tài (có thể lọc công khai)
        /// </summary>
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicProjects()
        {
            var projects = await _projectService.GetPublicProjectsAsync();
            return Ok(projects);
        }

        /// <summary>
        /// Lấy danh sách đề tài theo nhà nghiên cứu chính
        /// </summary>
        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetProjectsByOwner(string ownerId)
        {
            var projects = await _projectService.GetProjectsByOwnerAsync(ownerId);
            return Ok(projects);
        }

        /// <summary>
        /// Lấy chi tiết đề tài theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        /// <summary>
        /// Cập nhật đề tài
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] CreateProjectDto dto)
        {
            var success = await _projectService.UpdateProjectAsync(id, dto.Title, dto.Description, dto.Status);
            if (!success) return NotFound();
            return Ok(new { message = "Thông tin đề tài đã được cập nhật." });
        }

        /// <summary>
        /// (Tùy chọn) Xoá đề tài (chưa triển khai logic trong ProjectService)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();

            // Nếu thêm xóa sau này, cần bổ sung phương thức DeleteProjectAsync
            return BadRequest(new { message = "Tính năng xoá chưa được hỗ trợ." });
        }
    }
}