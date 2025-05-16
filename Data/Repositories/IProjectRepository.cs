using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(int id);
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Project>> GetByOwnerIdAsync(int ownerId);
        Task<IEnumerable<Project>> GetByMemberIdAsync(int memberId);
        Task<IEnumerable<Project>> GetByStatusAsync(string status);
        Task<IEnumerable<Project>> GetByResearchTopicIdAsync(int researchTopicId);
        Task<bool> CreateAsync(Project project);
        Task<bool> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddMemberAsync(int projectId, int userId, string role);
        Task<bool> RemoveMemberAsync(int projectId, int userId);
        Task<IEnumerable<User>> GetProjectMembersAsync(int projectId);
    }
}
