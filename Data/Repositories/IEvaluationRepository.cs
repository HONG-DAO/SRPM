using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public interface IEvaluationRepository
    {
        Task<Evaluation?> GetByIdAsync(int id);
        Task<IEnumerable<Evaluation>> GetAllAsync();
        Task<IEnumerable<Evaluation>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<Evaluation>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<Evaluation>> GetByEvaluatedByIdAsync(int userId);
        Task<IEnumerable<Evaluation>> GetByTypeAsync(string type);
        Task<bool> CreateAsync(Evaluation evaluation);
        Task<bool> UpdateAsync(Evaluation evaluation);
        Task<bool> DeleteAsync(int id);
    }
}
