using Microsoft.EntityFrameworkCore;
using SRPM.Data;
using SRPM.Models;

namespace SRPM.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo mới task cho đề tài
        /// </summary>
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            task.Id = Guid.NewGuid();
            task.CreatedAt = DateTime.UtcNow;
            task.Status = "Pending";
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// Cập nhật thông tin task (title, description, due date, status)
        /// </summary>
        public async Task<bool> UpdateTaskAsync(Guid taskId, string title, string description, DateTime? dueDate, string status)
        {
            var task = await _context.TaskItems.FindAsync(taskId);
            if (task == null) return false;

            task.Title = title;
            task.Description = description;
            if (dueDate.HasValue) task.DueDate = dueDate.Value;
            task.Status = status;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Giao task cho thành viên
        /// </summary>
        public async Task<bool> AssignTaskAsync(Guid taskId, string userId)
        {
            var task = await _context.TaskItems.FindAsync(taskId);
            if (task == null) return false;

            task.AssignedToId = userId;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Lấy danh sách task theo Project
        /// </summary>
        public async Task<List<TaskItem>> GetTasksByProjectAsync(Guid projectId)
        {
            return await _context.TaskItems
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Đánh dấu task hoàn thành
        /// </summary>
        public async Task<bool> MarkAsCompletedAsync(Guid taskId)
        {
            var task = await _context.TaskItems.FindAsync(taskId);
            if (task == null) return false;

            task.Status = "Completed";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
