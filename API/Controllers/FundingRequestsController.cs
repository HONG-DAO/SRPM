using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRPM.API.Models;
using SRPM.API.Services;
using System.Security.Claims;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FundingRequestsController : ControllerBase
    {
        private readonly IFundingRequestService _fundingRequestService;

        public FundingRequestsController(IFundingRequestService fundingRequestService)
        {
            _fundingRequestService = fundingRequestService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FundingRequestDto>> GetById(int id)
        {
            var fundingRequest = await _fundingRequestService.GetByIdAsync(id);
            if (fundingRequest == null)
                return NotFound();

            return Ok(fundingRequest);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FundingRequestDto>>> GetAll()
        {
            var fundingRequests = await _fundingRequestService.GetAllAsync();
            return Ok(fundingRequests);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<FundingRequestDto>>> GetByProjectId(int projectId)
        {
            var fundingRequests = await _fundingRequestService.GetByProjectIdAsync(projectId);
            return Ok(fundingRequests);
        }

        [HttpGet("requested-by/{userId}")]
        public async Task<ActionResult<IEnumerable<FundingRequestDto>>> GetByRequestedById(int userId)
        {
            var fundingRequests = await _fundingRequestService.GetByRequestedByIdAsync(userId);
            return Ok(fundingRequests);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<FundingRequestDto>>> GetByStatus(string status)
        {
            var fundingRequests = await _fundingRequestService.GetByStatusAsync(status);
            return Ok(fundingRequests);
        }

        [HttpPost]
        public async Task<ActionResult<FundingRequestDto>> Create([FromBody] CreateFundingRequestRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var fundingRequest = await _fundingRequestService.CreateAsync(request, userId);
            if (fundingRequest == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new { id = fundingRequest.Id }, fundingRequest);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateFundingRequestRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _fundingRequestService.UpdateAsync(id, request, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _fundingRequestService.DeleteAsync(id, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<ActionResult> Approve(int id, [FromBody] ApproveFundingRequestRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _fundingRequestService.ApproveAsync(id, request, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<ActionResult> Reject(int id, [FromBody] RejectFundingRequestRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var success = await _fundingRequestService.RejectAsync(id, request, userId);
            if (!success)
                return BadRequest();

            return NoContent();
        }
    }
}
