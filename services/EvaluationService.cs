using Microsoft.AspNetCore.Mvc;
using SRPM.Services;
using SRPM.Models;
using SRPM.Dto;

namespace SRPM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationController : ControllerBase
    {
        private readonly EvaluationService _evaluationService;

        public EvaluationController(EvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        /// <summary>
        /// Gửi đánh giá mới cho một đề tài
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateEvaluation([FromBody] EvaluationDto dto)
        {
            var evaluation = new Evaluation
            {
                ProjectId = dto.ProjectId,
                EvaluatorId = dto.EvaluatorId,
                Score = dto.Score,
                Comment = dto.Comment
            };

            var created = await _evaluationService.CreateEvaluationAsync(evaluation);
            return Ok(new { message = "Đánh giá đã được ghi nhận.", evaluationId = created.Id });
        }

        /// <summary>
        /// Lấy danh sách đánh giá của một đề tài
        /// </summary>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetEvaluationsByProject(Guid projectId)
        {
            var evaluations = await _evaluationService.GetEvaluationsByProjectAsync(projectId);
            return Ok(evaluations);
        }

        /// <summary>
        /// Cập nhật đánh giá
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvaluation(Guid id, [FromBody] EvaluationDto dto)
        {
            var success = await _evaluationService.UpdateEvaluationAsync(id, dto.Score, dto.Comment);
            if (!success) return NotFound();
            return Ok(new { message = "Đánh giá đã được cập nhật." });
        }

        /// <summary>
        /// Xóa đánh giá
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluation(Guid id)
        {
            var success = await _evaluationService.DeleteEvaluationAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Đã xóa đánh giá." });
        }
    }
}
