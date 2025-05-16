using SRPM.API.Models;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public interface IEvaluationService
    {
        Task<EvaluationDto?> GetByIdAsync(int id);
        Task<IEnumerable<EvaluationDto>> GetAllAsync();
        Task<IEnumerable<EvaluationDto>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<EvaluationDto>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<EvaluationDto>> GetByEvaluatedByIdAsync(int userId);
        Task<IEnumerable<EvaluationDto>> GetByTypeAsync(string type);
        Task<EvaluationDto?> CreateAsync(CreateEvaluationRequest request, int userId);
        Task<bool> UpdateAsync(int id, UpdateEvaluationRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
