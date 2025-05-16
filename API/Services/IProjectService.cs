using SRPM.API.Models;

namespace SRPM.API.Services
{
    public interface IProjectService
    {
        Task<ProjectDto?> GetByIdAsync(int id, int userId);
        Task<IEnumerable<ProjectDto>> GetAllAsync(int userId);
        Task<IEnumerable<ProjectDto>> GetByOwnerIdAsync(int ownerId);
        Task<IEnumerable<ProjectDto>> GetByMemberIdAsync(int memberId);
        Task<IEnumerable<ProjectDto>> GetByStatusAsync(string status);
        Task<IEnumerable<ProjectDto>> GetByResearchTopicIdAsync(int researchTopicId);
        Task<ProjectDto?> CreateAsync(CreateProjectRequest request, int ownerId);
        Task<bool> UpdateAsync(int id, UpdateProjectRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<bool> AddMemberAsync(int projectId, AddProjectMemberRequest request, int userId);
        Task<bool> RemoveMemberAsync(int projectId, int memberId, int userId);
        Task<IEnumerable<UserDto>> GetProjectMembersAsync(int projectId);
    }
}
