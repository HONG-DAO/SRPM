using SRPM.API.Models;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public interface IResearchTopicService
    {
        Task<ResearchTopicDto?> GetByIdAsync(int id);
        Task<IEnumerable<ResearchTopicDto>> GetAllAsync();
        Task<IEnumerable<ResearchTopicDto>> GetActiveAsync();
        Task<IEnumerable<ResearchTopicDto>> GetByCreatedByIdAsync(int userId);
        Task<ResearchTopicDto?> CreateAsync(CreateResearchTopicRequest request, int userId);
        Task<bool> UpdateAsync(int id, UpdateResearchTopicRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> ToggleActiveStatusAsync(int id, int userId);
    }
}
