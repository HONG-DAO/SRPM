using SRPM.API.Models;
using SRPM.Data.Entities;
using SRPM.Data.Repositories;

namespace SRPM.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            INotificationRepository notificationRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<ProjectDto?> GetByIdAsync(int id, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return null;

            // Check if user is a member of the project or the owner
            bool isMember = project.OwnerId == userId || project.ProjectMembers.Any(pm => pm.UserId == userId);
            
            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                OwnerName = project.Owner.Name,
                ResearchTopicId = project.ResearchTopicId,
                ResearchTopicTitle = project.ResearchTopic?.Title,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };

            // If user is not a member, return limited information
            if (!isMember)
            {
                return new ProjectDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    Status = project.Status,
                    OwnerId = project.OwnerId,
                    OwnerName = project.Owner.Name
                };
            }

            // Return full information for members
            return projectDto;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync(int userId)
        {
            var projects = await _projectRepository.GetAllAsync();
            var projectDtos = new List<ProjectDto>();

            foreach (var project in projects)
            {
                // Check if user is a member of the project or the owner
                bool isMember = project.OwnerId == userId || project.ProjectMembers.Any(pm => pm.UserId == userId);

                var projectDto = new ProjectDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    Status = project.Status,
                    OwnerId = project.OwnerId,
                    OwnerName = project.Owner.Name
                };

                // Add more details if user is a member
                if (isMember)
                {
                    projectDto.StartDate = project.StartDate;
                    projectDto.EndDate = project.EndDate;
                    projectDto.ResearchTopicId = project.ResearchTopicId;
                    projectDto.ResearchTopicTitle = project.ResearchTopic?.Title;
                    projectDto.CreatedAt = project.CreatedAt;
                    projectDto.UpdatedAt = project.UpdatedAt;
                }

                projectDtos.Add(projectDto);
            }

            return projectDtos;
        }

        public async Task<IEnumerable<ProjectDto>> GetByOwnerIdAsync(int ownerId)
        {
            var projects = await _projectRepository.GetByOwnerIdAsync(ownerId);
            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner.Name,
                ResearchTopicId = p.ResearchTopicId,
                ResearchTopicTitle = p.ResearchTopic?.Title,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<IEnumerable<ProjectDto>> GetByMemberIdAsync(int memberId)
        {
            var projects = await _projectRepository.GetByMemberIdAsync(memberId);
            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner.Name,
                ResearchTopicId = p.ResearchTopicId,
                ResearchTopicTitle = p.ResearchTopic?.Title,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<IEnumerable<ProjectDto>> GetByStatusAsync(string status)
        {
            var projects = await _projectRepository.GetByStatusAsync(status);
            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner.Name,
                ResearchTopicId = p.ResearchTopicId,
                ResearchTopicTitle = p.ResearchTopic?.Title,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<IEnumerable<ProjectDto>> GetByResearchTopicIdAsync(int researchTopicId)
        {
            var projects = await _projectRepository.GetByResearchTopicIdAsync(researchTopicId);
            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner.Name,
                ResearchTopicId = p.ResearchTopicId,
                ResearchTopicTitle = p.ResearchTopic?.Title,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<ProjectDto?> CreateAsync(CreateProjectRequest request, int ownerId)
        {
            var owner = await _userRepository.GetByIdAsync(ownerId);
            if (owner == null)
                return null;

            // Check if user has Principal Investigator role
            var roles = await _userRepository.GetUserRolesAsync(ownerId);
            if (!roles.Any(r => r.Name == "PrincipalInvestigator"))
                return null;

            var project = new Project
            {
                Title = request.Title,
                Description = request.Description,
                Objectives = request.Objectives,
                ExpectedOutcomes = request.ExpectedOutcomes,
                Status = "Draft",
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                OwnerId = ownerId,
                ResearchTopicId = request.ResearchTopicId
            };

            var success = await _projectRepository.CreateAsync(project);
            if (!success)
                return null;

            // Add owner as a member with PI role
            await _projectRepository.AddMemberAsync(project.Id, ownerId, "PrincipalInvestigator");

            return new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                OwnerName = owner.Name,
                ResearchTopicId = project.ResearchTopicId,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateProjectRequest request, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return false;

            // Check if user is the owner or has admin role
            if (project.OwnerId != userId)
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            project.Title = request.Title;
            project.Description = request.Description;
            project.Objectives = request.Objectives;
            project.ExpectedOutcomes = request.ExpectedOutcomes;
            project.Status = request.Status;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.ResearchTopicId = request.ResearchTopicId;

            return await _projectRepository.UpdateAsync(project);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return false;

            // Check if user is the owner or has admin role
            if (project.OwnerId != userId)
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            return await _projectRepository.DeleteAsync(id);
        }

        public async Task<bool> AddMemberAsync(int projectId, AddProjectMemberRequest request, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                return false;

            // Check if user is the owner or has admin role
            if (project.OwnerId != userId)
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            // Check if member exists
            var member = await _userRepository.GetByIdAsync(request.UserId);
            if (member == null)
                return false;

            var success = await _projectRepository.AddMemberAsync(projectId, request.UserId, request.Role);
            if (success)
            {
                // Create notification for the new member
                var notification = new Notification
                {
                    Title = "Added to Project",
                    Message = $"You have been added to the project '{project.Title}' as {request.Role}.",
                    Type = "Info",
                    RelatedEntityType = "Project",
                    RelatedEntityId = projectId,
                    UserId = request.UserId
                };

                await _notificationRepository.CreateAsync(notification);
            }

            return success;
        }

        public async Task<bool> RemoveMemberAsync(int projectId, int memberId, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                return false;

            // Check if user is the owner or has admin role
            if (project.OwnerId != userId)
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            // Cannot remove the owner
            if (project.OwnerId == memberId)
                return false;

            var success = await _projectRepository.RemoveMemberAsync(projectId, memberId);
            if (success)
            {
                // Create notification for the removed member
                var notification = new Notification
                {
                    Title = "Removed from Project",
                    Message = $"You have been removed from the project '{project.Title}'.",
                    Type = "Info",
                    RelatedEntityType = "Project",
                    RelatedEntityId = projectId,
                    UserId = memberId
                };

                await _notificationRepository.CreateAsync(notification);
            }

            return success;
        }

        public async Task<IEnumerable<UserDto>> GetProjectMembersAsync(int projectId)
        {
            var members = await _projectRepository.GetProjectMembersAsync(projectId);
            var memberDtos = new List<UserDto>();

            foreach (var member in members)
            {
                var roles = await _userRepository.GetUserRolesAsync(member.Id);

                memberDtos.Add(new UserDto
                {
                    Id = member.Id,
                    Email = member.Email,
                    Name = member.Name,
                    AvatarUrl = member.AvatarUrl,
                    Roles = roles.Select(r => r.Name).ToList()
                });
            }

            return memberDtos;
        }
    }
}
