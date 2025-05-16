using Microsoft.EntityFrameworkCore;
using SRPM.Data.Entities;

namespace SRPM.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ResearchTopic)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ResearchTopic)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByOwnerIdAsync(int ownerId)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ResearchTopic)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByMemberIdAsync(int memberId)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ResearchTopic)
                .Where(p => p.ProjectMembers.Any(pm => pm.UserId == memberId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByStatusAsync(string status)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ResearchTopic)
                .Where(p => p.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByResearchTopicIdAsync(int researchTopicId)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ResearchTopic)
                .Where(p => p.ResearchTopicId == researchTopicId)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Project project)
        {
            _context.Projects.Add(project);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Project project)
        {
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(project);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            _context.Projects.Remove(project);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddMemberAsync(int projectId, int userId, string role)
        {
            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            };

            _context.ProjectMembers.Add(projectMember);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveMemberAsync(int projectId, int userId)
        {
            var projectMember = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (projectMember == null)
                return false;

            _context.ProjectMembers.Remove(projectMember);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<User>> GetProjectMembersAsync(int projectId)
        {
            return await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Select(pm => pm.User)
                .ToListAsync();
        }
    }
}
