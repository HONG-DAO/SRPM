using Microsoft.AspNetCore.Mvc;
using SRPM.Services;
using SRPM.Models;
using SRPM.Dto;

namespace SRPM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundingController : ControllerBase
    {
        private readonly FundingService _fundingService;

        public FundingController(FundingService fundingService)
        {
            _fundingService = fundingService;
        }

        /// <summary>
        /// Gửi yêu cầu tài trợ mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SubmitFundingRequest([FromBody] FundingRequestDto dto)
        {
            var request = new FundingRequest
            {
                ProjectId = dto.ProjectId,
                RequestedById = dto.RequestedById,
                Amount = dto.Amount,
                Purpose = dto.Purpose
            };

            var created = await _fundingService.SubmitRequestAsync(request);
            return Ok(new { message = "Yêu cầu tài trợ đã được gửi.", requestId = created.Id });
        }

        /// <summary>
        /// Lấy danh sách yêu cầu tài trợ theo dự án
        /// </summary>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetFundingRequestsByProject(Guid projectId)
        {
            var requests = await _fundingService.GetByProjectAsync(projectId);
            return Ok(requests);
        }

        /// <summary>
        /// Lấy chi tiết 1 yêu cầu tài trợ
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFundingRequestById(Guid id)
        {
            var request = await _fundingService.GetByIdAsync(id);
            if (request == null) return NotFound();

            return Ok(request);
        }

        /// <summary>
        /// Duyệt hoặc từ chối yêu cầu tài trợ
        /// </summary>
        [HttpPut("{id}/review")]
        public async Task<IActionResult> ReviewFundingRequest(Guid id, [FromBody] ReviewFundingDto dto)
        {
            var success = await _fundingService.ReviewRequestAsync(id, dto.Status, dto.Note);
            if (!success) return NotFound();

            return Ok(new { message = $"Yêu cầu tài trợ đã được {dto.Status.ToLower()}." });
        }
    }

    public class ReviewFundingDto
    {
        public string Status { get; set; } = "Approved"; // or Rejected
        public string Note { get; set; } = string.Empty;
    }
}
