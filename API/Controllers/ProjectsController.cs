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
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var project = await _projectService.GetByIdAsync(id, userId);
            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var projects = await _projectService.GetAllAsync(userId);
            return Ok(projects);
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByOwnerId(int ownerId)
        {
            var projects = await _projectService.GetByOwnerIdAsync(ownerId);
            return Ok(projects);
        }

        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByMemberId(int memberId)
        {
            var projects = await _projectService.GetByMemberIdAsync(memberId);
            return Ok(projects);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByStatus(string status)
        {
            var projects = await _projectService.GetByStatusAsync(status);
            return Ok(projects);
        }

        [HttpGet("research-topic/{researchTopicId}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByResearchTopicId(int researchTopicId)
        {
            var projects = await _projectService.GetByResearchTopicIdAsync(researchTopicId);
            return Ok(projects);
        }

        [HttpPost]
        [Authorize(Roles = "PrincipalInvestigator")]
        public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var project = await _projectService.CreateAsync(request, userId);
            if (project == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateProjectRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _projectService.UpdateAsync(id, request, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _projectService.DeleteAsync(id, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpPost("{id}/members")]
        public async Task<ActionResult> AddMember(int id, [FromBody] AddProjectMemberRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _projectService.AddMemberAsync(id, request, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}/members/{memberId}")]
        public async Task<ActionResult> RemoveMember(int id, int memberId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _projectService.RemoveMemberAsync(id, memberId, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetMembers(int id)
        {
            var members = await _projectService.GetProjectMembersAsync(id);
            return Ok(members);
        }
    }
}
