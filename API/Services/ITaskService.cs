using SRPM.API.Models;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public interface ITaskService
    {
        Task<TaskDto?> GetByIdAsync(int id);
        Task<IEnumerable<TaskDto>> GetAllAsync();
        Task<IEnumerable<TaskDto>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<TaskDto>> GetByAssignedToIdAsync(int userId);
        Task<IEnumerable<TaskDto>> GetByStatusAsync(string status);
        Task<IEnumerable<TaskDto>> GetMilestonesAsync(int projectId);
        Task<TaskDto?> CreateAsync(CreateTaskRequest request, int userId);
        Task<bool> UpdateAsync(int id, UpdateTaskRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> UpdateStatusAsync(int id, UpdateTaskStatusRequest request, int userId);
    }
}
