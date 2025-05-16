using TaskEntity = SRPM.Data.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace SRPM.Data.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetByIdAsync(int id);
        Task<IEnumerable<TaskEntity>> GetAllAsync();
        Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<TaskEntity>> GetByAssignedToIdAsync(int userId);
        Task<IEnumerable<TaskEntity>> GetByStatusAsync(string status);
        Task<IEnumerable<TaskEntity>> GetMilestonesAsync(int projectId);
        Task<bool> CreateAsync(TaskEntity task);
        Task<bool> UpdateAsync(TaskEntity task);
        Task<bool> DeleteAsync(int id);
    }
}
