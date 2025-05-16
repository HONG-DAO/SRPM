using SRPM.API.Models;
using SRPM.Data.Entities;
using SRPM.Data.Repositories;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public class FundingRequestService : IFundingRequestService
    {
        private readonly IFundingRequestRepository _fundingRequestRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public FundingRequestService(
            IFundingRequestRepository fundingRequestRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            INotificationRepository notificationRepository)
        {
            _fundingRequestRepository = fundingRequestRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<FundingRequestDto?> GetByIdAsync(int id)
        {
            var fundingRequest = await _fundingRequestRepository.GetByIdAsync(id);
            if (fundingRequest == null)
                return null;

            return MapToDto(fundingRequest);
        }

        public async Task<IEnumerable<FundingRequestDto>> GetAllAsync()
        {
            var fundingRequests = await _fundingRequestRepository.GetAllAsync();
            return fundingRequests.Select(MapToDto);
        }

        public async Task<IEnumerable<FundingRequestDto>> GetByProjectIdAsync(int projectId)
        {
            var fundingRequests = await _fundingRequestRepository.GetByProjectIdAsync(projectId);
            return fundingRequests.Select(MapToDto);
        }

        public async Task<IEnumerable<FundingRequestDto>> GetByRequestedByIdAsync(int userId)
        {
            var fundingRequests = await _fundingRequestRepository.GetByRequestedByIdAsync(userId);
            return fundingRequests.Select(MapToDto);
        }

        public async Task<IEnumerable<FundingRequestDto>> GetByStatusAsync(string status)
        {
            var fundingRequests = await _fundingRequestRepository.GetByStatusAsync(status);
            return fundingRequests.Select(MapToDto);
        }

        public async Task<FundingRequestDto?> CreateAsync(CreateFundingRequestRequest request, int userId)
        {
            // Check if project exists
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
                return null;

            // Check if user is a member of the project or the owner
            bool isMember = project.OwnerId == userId || project.ProjectMembers.Any(pm => pm.UserId == userId);
            if (!isMember)
                return null;

            var fundingRequest = new FundingRequest
            {
                Title = request.Title,
                Description = request.Description,
                Amount = request.Amount,
                Status = "Pending",
                Purpose = request.Purpose,
                JustificationDocumentUrl = request.JustificationDocumentUrl,
                ProjectId = request.ProjectId,
                RequestedById = userId
            };

            var success = await _fundingRequestRepository.CreateAsync(fundingRequest);
            if (!success)
                return null;

            // Create notification for project owner if the requester is not the owner
            if (project.OwnerId != userId)
            {
                var notification = new Notification
                {
                    Title = "New Funding Request",
                    Message = $"A new funding request has been submitted for project '{project.Title}': {fundingRequest.Title}",
                    Type = "Info",
                    RelatedEntityType = "FundingRequest",
                    RelatedEntityId = fundingRequest.Id,
                    UserId = project.OwnerId
                };

                await _notificationRepository.CreateAsync(notification);
            }

            // Create notifications for staff members
            var staffMembers = await _userRepository.GetByRoleAsync("Staff");
            foreach (var staff in staffMembers)
            {
                var notification = new Notification
                {
                    Title = "New Funding Request",
                    Message = $"A new funding request has been submitted for project '{project.Title}': {fundingRequest.Title}",
                    Type = "Info",
                    RelatedEntityType = "FundingRequest",
                    RelatedEntityId = fundingRequest.Id,
                    UserId = staff.Id
                };

                await _notificationRepository.CreateAsync(notification);
            }

            return MapToDto(fundingRequest);
        }

        public async Task<bool> UpdateAsync(int id, UpdateFundingRequestRequest request, int userId)
        {
            var fundingRequest = await _fundingRequestRepository.GetByIdAsync(id);
            if (fundingRequest == null)
                return false;

            // Only the requester can update the funding request
            if (fundingRequest.RequestedById != userId)
                return false;

            // Can only update if status is Pending
            if (fundingRequest.Status != "Pending")
                return false;

            fundingRequest.Title = request.Title;
            fundingRequest.Description = request.Description;
            fundingRequest.Amount = request.Amount;
            fundingRequest.Purpose = request.Purpose;
            fundingRequest.JustificationDocumentUrl = request.JustificationDocumentUrl;

            return await _fundingRequestRepository.UpdateAsync(fundingRequest);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var fundingRequest = await _fundingRequestRepository.GetByIdAsync(id);
            if (fundingRequest == null)
                return false;

            // Only the requester can delete the funding request
            if (fundingRequest.RequestedById != userId)
            {
                // Or an admin
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            // Can only delete if status is Pending
            if (fundingRequest.Status != "Pending")
                return false;

            return await _fundingRequestRepository.DeleteAsync(id);
        }

        public async Task<bool> ApproveAsync(int id, ApproveFundingRequestRequest request, int userId)
        {
            var fundingRequest = await _fundingRequestRepository.GetByIdAsync(id);
            if (fundingRequest == null)
                return false;

            // Check if user has Staff or Admin role
            var roles = await _userRepository.GetUserRolesAsync(userId);
            if (!roles.Any(r => r.Name == "Staff" || r.Name == "Admin"))
                return false;

            // Can only approve if status is Pending
            if (fundingRequest.Status != "Pending")
                return false;

            fundingRequest.Status = "Approved";
            fundingRequest.ApprovedById = userId;
            fundingRequest.ApprovedAt = DateTime.UtcNow;

            var success = await _fundingRequestRepository.UpdateAsync(fundingRequest);
            if (success)
            {
                // Create notification for the requester
                var notification = new Notification
                {
                    Title = "Funding Request Approved",
                    Message = $"Your funding request '{fundingRequest.Title}' has been approved.",
                    Type = "Success",
                    RelatedEntityType = "FundingRequest",
                    RelatedEntityId = fundingRequest.Id,
                    UserId = fundingRequest.RequestedById
                };

                await _notificationRepository.CreateAsync(notification);

                // Create notification for project owner if different from requester
                var project = await _projectRepository.GetByIdAsync(fundingRequest.ProjectId);
                if (project != null && project.OwnerId != fundingRequest.RequestedById)
                {
                    var ownerNotification = new Notification
                    {
                        Title = "Funding Request Approved",
                        Message = $"A funding request for project '{project.Title}' has been approved: {fundingRequest.Title}",
                        Type = "Success",
                        RelatedEntityType = "FundingRequest",
                        RelatedEntityId = fundingRequest.Id,
                        UserId = project.OwnerId
                    };

                    await _notificationRepository.CreateAsync(ownerNotification);
                }
            }

            return success;
        }

        public async Task<bool> RejectAsync(int id, RejectFundingRequestRequest request, int userId)
        {
            var fundingRequest = await _fundingRequestRepository.GetByIdAsync(id);
            if (fundingRequest == null)
                return false;

            // Check if user has Staff or Admin role
            var roles = await _userRepository.GetUserRolesAsync(userId);
            if (!roles.Any(r => r.Name == "Staff" || r.Name == "Admin"))
                return false;

            // Can only reject if status is Pending
            if (fundingRequest.Status != "Pending")
                return false;

            fundingRequest.Status = "Rejected";

            var success = await _fundingRequestRepository.UpdateAsync(fundingRequest);
            if (success)
            {
                // Create notification for the requester
                var notification = new Notification
                {
                    Title = "Funding Request Rejected",
                    Message = $"Your funding request '{fundingRequest.Title}' has been rejected. Reason: {request.Reason}",
                    Type = "Error",
                    RelatedEntityType = "FundingRequest",
                    RelatedEntityId = fundingRequest.Id,
                    UserId = fundingRequest.RequestedById
                };

                await _notificationRepository.CreateAsync(notification);

                // Create notification for project owner if different from requester
                var project = await _projectRepository.GetByIdAsync(fundingRequest.ProjectId);
                if (project != null && project.OwnerId != fundingRequest.RequestedById)
                {
                    var ownerNotification = new Notification
                    {
                        Title = "Funding Request Rejected",
                        Message = $"A funding request for project '{project.Title}' has been rejected: {fundingRequest.Title}",
                        Type = "Error",
                        RelatedEntityType = "FundingRequest",
                        RelatedEntityId = fundingRequest.Id,
                        UserId = project.OwnerId
                    };

                    await _notificationRepository.CreateAsync(ownerNotification);
                }
            }

            return success;
        }

        private FundingRequestDto MapToDto(FundingRequest fundingRequest)
        {
            return new FundingRequestDto
            {
                Id = fundingRequest.Id,
                Title = fundingRequest.Title,
                Description = fundingRequest.Description,
                Amount = fundingRequest.Amount,
                Status = fundingRequest.Status,
                Purpose = fundingRequest.Purpose,
                JustificationDocumentUrl = fundingRequest.JustificationDocumentUrl,
                ProjectId = fundingRequest.ProjectId,
                ProjectTitle = fundingRequest.Project?.Title,
                RequestedById = fundingRequest.RequestedById,
                RequestedByName = fundingRequest.RequestedBy?.Name,
                ApprovedById = fundingRequest.ApprovedById,
                ApprovedAt = fundingRequest.ApprovedAt,
                CreatedAt = fundingRequest.CreatedAt,
                UpdatedAt = fundingRequest.UpdatedAt
            };
        }
    }
}
