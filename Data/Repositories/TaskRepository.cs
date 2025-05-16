using Microsoft.EntityFrameworkCore;
using TaskEntity = SRPM.Data.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace SRPM.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(int projectId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetByAssignedToIdAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Where(t => t.AssignedToId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetByStatusAsync(string status)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetMilestonesAsync(int projectId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Where(t => t.ProjectId == projectId && t.IsMilestone)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(TaskEntity task)
        {
            _context.Tasks.Add(task);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(TaskEntity task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            _context.Tasks.Update(task);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
