using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public interface IResearchTopicRepository
    {
        Task<ResearchTopic?> GetByIdAsync(int id);
        Task<IEnumerable<ResearchTopic>> GetAllAsync();
        Task<IEnumerable<ResearchTopic>> GetActiveAsync();
        Task<IEnumerable<ResearchTopic>> GetByCreatedByIdAsync(int userId);
        Task<bool> CreateAsync(ResearchTopic researchTopic);
        Task<bool> UpdateAsync(ResearchTopic researchTopic);
        Task<bool> DeleteAsync(int id);
    }
}
