using SRPM.API.Models;
using SRPM.Data.Entities;
using SRPM.Data.Repositories;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public class ResearchTopicService : IResearchTopicService
    {
        private readonly IResearchTopicRepository _researchTopicRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public ResearchTopicService(
            IResearchTopicRepository researchTopicRepository,
            IUserRepository userRepository,
            INotificationRepository notificationRepository)
        {
            _researchTopicRepository = researchTopicRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<ResearchTopicDto?> GetByIdAsync(int id)
        {
            var researchTopic = await _researchTopicRepository.GetByIdAsync(id);
            if (researchTopic == null)
                return null;

            return MapToDto(researchTopic);
        }

        public async Task<IEnumerable<ResearchTopicDto>> GetAllAsync()
        {
            var researchTopics = await _researchTopicRepository.GetAllAsync();
            return researchTopics.Select(MapToDto);
        }

        public async Task<IEnumerable<ResearchTopicDto>> GetActiveAsync()
        {
            var researchTopics = await _researchTopicRepository.GetActiveAsync();
            return researchTopics.Select(MapToDto);
        }

        public async Task<IEnumerable<ResearchTopicDto>> GetByCreatedByIdAsync(int userId)
        {
            var researchTopics = await _researchTopicRepository.GetByCreatedByIdAsync(userId);
            return researchTopics.Select(MapToDto);
        }

        public async Task<ResearchTopicDto?> CreateAsync(CreateResearchTopicRequest request, int userId)
        {
            // Check if user has HostInstitution role
            var roles = await _userRepository.GetUserRolesAsync(userId);
            if (!roles.Any(r => r.Name == "HostInstitution" || r.Name == "Admin"))
                return null;

            var researchTopic = new ResearchTopic
            {
                Title = request.Title,
                Description = request.Description,
                Objectives = request.Objectives,
                ExpectedOutcomes = request.ExpectedOutcomes,
                Requirements = request.Requirements,
                AvailableFunding = request.AvailableFunding,
                IsActive = true,
                CreatedById = userId
            };

            var success = await _researchTopicRepository.CreateAsync(researchTopic);
            if (!success)
                return null;

            // Create notifications for all Principal Investigators
            var principalInvestigators = await _userRepository.GetByRoleAsync("PrincipalInvestigator");
            foreach (var pi in principalInvestigators)
            {
                var notification = new Notification
                {
                    Title = "New Research Topic",
                    Message = $"A new research topic has been published: {researchTopic.Title}",
                    Type = "Info",
                    RelatedEntityType = "ResearchTopic",
                    RelatedEntityId = researchTopic.Id,
                    UserId = pi.Id
                };

                await _notificationRepository.CreateAsync(notification);
            }

            return MapToDto(researchTopic);
        }

        public async Task<bool> UpdateAsync(int id, UpdateResearchTopicRequest request, int userId)
        {
            var researchTopic = await _researchTopicRepository.GetByIdAsync(id);
            if (researchTopic == null)
                return false;

            // Check if user is the creator or has admin role
            if (researchTopic.CreatedById != userId)
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            researchTopic.Title = request.Title;
            researchTopic.Description = request.Description;
            researchTopic.Objectives = request.Objectives;
            researchTopic.ExpectedOutcomes = request.ExpectedOutcomes;
            researchTopic.Requirements = request.Requirements;
            researchTopic.AvailableFunding = request.AvailableFunding;

            return await _researchTopicRepository.UpdateAsync(researchTopic);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var researchTopic = await _researchTopicRepository.GetByIdAsync(id);
            if (researchTopic == null)
                return false;

            // Check if user is the creator or has admin role
            if (researchTopic.CreatedById != userId)
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            return await _researchTopicRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleActiveStatusAsync(int id, int userId)
        {
            var researchTopic = await _researchTopicRepository.GetByIdAsync(id);
            if (researchTopic == null)
                return false;

            // Check if user is the creator or has admin role
            if (researchTopic.CreatedById != userId)
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            researchTopic.IsActive = !researchTopic.IsActive;
            return await _researchTopicRepository.UpdateAsync(researchTopic);
        }

        private ResearchTopicDto MapToDto(ResearchTopic researchTopic)
        {
            return new ResearchTopicDto
            {
                Id = researchTopic.Id,
                Title = researchTopic.Title,
                Description = researchTopic.Description,
                Objectives = researchTopic.Objectives,
                ExpectedOutcomes = researchTopic.ExpectedOutcomes,
                Requirements = researchTopic.Requirements,
                AvailableFunding = researchTopic.AvailableFunding,
                IsActive = researchTopic.IsActive,
                CreatedById = researchTopic.CreatedById,
                CreatedByName = researchTopic.CreatedBy?.Name,
                CreatedAt = researchTopic.CreatedAt,
                UpdatedAt = researchTopic.UpdatedAt
            };
        }
    }
}
