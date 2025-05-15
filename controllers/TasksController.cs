using Microsoft.AspNetCore.Mvc;
using SRPM.Services;
using SRPM.Models;
using SRPM.Dto;

namespace SRPM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Tạo task mới cho đề tài
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItemDto dto)
        {
            var task = new TaskItem
            {
                ProjectId = dto.ProjectId,
                Title = dto.Title,
                Description = dto.Description,
                AssignedToId = dto.AssignedToId,
                DueDate = dto.DueDate,
                Status = dto.Status ?? "Pending"
            };

            var result = await _taskService.CreateTaskAsync(task);
            return Ok(new { message = "Đã tạo task thành công.", taskId = result.Id });
        }

        /// <summary>
        /// Lấy danh sách task theo đề tài
        /// </summary>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(Guid projectId)
        {
            var tasks = await _taskService.GetTasksByProjectAsync(projectId);
            return Ok(tasks);
        }

        /// <summary>
        /// Cập nhật nội dung và tiến độ task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskItemDto dto)
        {
            var success = await _taskService.UpdateTaskAsync(id, dto.Title, dto.Description, dto.DueDate, dto.Status ?? "Pending");
            if (!success) return NotFound();
            return Ok(new { message = "Đã cập nhật task." });
        }

        /// <summary>
        /// Đánh dấu task đã hoàn thành
        /// </summary>
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkTaskCompleted(Guid id)
        {
            var success = await _taskService.MarkAsCompletedAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Task đã hoàn thành." });
        }

        /// <summary>
        /// Xoá task
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var success = await _taskService.DeleteTaskAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Đã xoá task." });
        }
    }
}
