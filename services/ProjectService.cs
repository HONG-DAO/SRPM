using Microsoft.EntityFrameworkCore;
using SRPM.Data;
using SRPM.Models;

namespace SRPM.Services
{
    public class ProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo mới đề tài nghiên cứu
        /// </summary>
        public async Task<Project> CreateProjectAsync(Project project)
        {
            project.Id = Guid.NewGuid();
            project.CreatedAt = DateTime.UtcNow;
            project.Status = "Draft";
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        /// <summary>
        /// Lấy danh sách đề tài của một nhà nghiên cứu chính (theo OwnerId)
        /// </summary>
        public async Task<List<Project>> GetProjectsByOwnerAsync(string ownerId)
        {
            return await _context.Projects
                .Where(p => p.OwnerId == ownerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy đề tài theo ID
        /// </summary>
        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _context.Projects.FindAsync(id);
        }

        /// <summary>
        /// Cập nhật thông tin đề tài
        /// </summary>
        public async Task<bool> UpdateProjectAsync(Guid id, string title, string description, string status)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            project.Title = title;
            project.Description = description;
            project.Status = status;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Lấy các đề tài công khai cơ bản (chỉ tiêu đề, trạng thái...)
        /// </summary>
        public async Task<List<Project>> GetPublicProjectsAsync()
        {
            return await _context.Projects
                .Select(p => new Project
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }
    }
}
