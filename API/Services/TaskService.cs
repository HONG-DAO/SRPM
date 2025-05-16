using SRPM.API.Models;
using SRPM.Data.Repositories;
using Task = System.Threading.Tasks.Task;
using TaskEntity = SRPM.Data.Entities.Task;

namespace SRPM.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            INotificationRepository notificationRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<TaskDto?> GetByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return null;

            return MapToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetAllAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<TaskDto>> GetByProjectIdAsync(int projectId)
        {
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<TaskDto>> GetByAssignedToIdAsync(int userId)
        {
            var tasks = await _taskRepository.GetByAssignedToIdAsync(userId);
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<TaskDto>> GetByStatusAsync(string status)
        {
            var tasks = await _taskRepository.GetByStatusAsync(status);
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<TaskDto>> GetMilestonesAsync(int projectId)
        {
            var tasks = await _taskRepository.GetMilestonesAsync(projectId);
            return tasks.Select(MapToDto);
        }

        public async Task<TaskDto?> CreateAsync(CreateTaskRequest request, int userId)
        {
            // Check if project exists
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
                return null;

            // Check if user is a member of the project or the owner
            bool isMember = project.OwnerId == userId || project.ProjectMembers.Any(pm => pm.UserId == userId);
            if (!isMember)
                return null;

            // Check if assigned user exists and is a member of the project
            if (request.AssignedToId.HasValue)
            {
                var assignedUser = await _userRepository.GetByIdAsync(request.AssignedToId.Value);
                if (assignedUser == null)
                    return null;

                bool isAssignedUserMember = project.ProjectMembers.Any(pm => pm.UserId == request.AssignedToId.Value);
                if (!isAssignedUserMember && project.OwnerId != request.AssignedToId.Value)
                    return null;
            }

            var task = new TaskEntity
            {
                Title = request.Title,
                Description = request.Description,
                Status = "NotStarted",
                StartDate = request.StartDate,
                DueDate = request.DueDate,
                ProjectId = request.ProjectId,
                AssignedToId = request.AssignedToId,
                IsMilestone = request.IsMilestone,
                AttachmentUrls = request.AttachmentUrls
            };

            var success = await _taskRepository.CreateAsync(task);
            if (!success)
                return null;

            // Create notification for assigned user
            if (request.AssignedToId.HasValue)
            {
                var notification = new SRPM.Data.Entities.Notification
                {
                    Title = "New Task Assigned",
                    Message = $"You have been assigned a new task: {task.Title}",
                    Type = "Info",
                    RelatedEntityType = "Task",
                    RelatedEntityId = task.Id,
                    UserId = request.AssignedToId.Value
                };

                await _notificationRepository.CreateAsync(notification);
            }

            return MapToDto(task);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTaskRequest request, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return false;

            // Check if user is a member of the project or the owner
            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
                return false;

            bool isMember = project.OwnerId == userId || project.ProjectMembers.Any(pm => pm.UserId == userId);
            if (!isMember)
                return false;

            // Check if assigned user exists and is a member of the project
            if (request.AssignedToId.HasValue)
            {
                var assignedUser = await _userRepository.GetByIdAsync(request.AssignedToId.Value);
                if (assignedUser == null)
                    return false;

                bool isAssignedUserMember = project.ProjectMembers.Any(pm => pm.UserId == request.AssignedToId.Value);
                if (!isAssignedUserMember && project.OwnerId != request.AssignedToId.Value)
                    return false;
            }

            // If the assigned user is changing, create a notification
            if (task.AssignedToId != request.AssignedToId && request.AssignedToId.HasValue)
            {
                var notification = new SRPM.Data.Entities.Notification
                {
                    Title = "Task Assigned",
                    Message = $"You have been assigned to the task: {task.Title}",
                    Type = "Info",
                    RelatedEntityType = "Task",
                    RelatedEntityId = task.Id,
                    UserId = request.AssignedToId.Value
                };

                await _notificationRepository.CreateAsync(notification);
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.StartDate = request.StartDate;
            task.DueDate = request.DueDate;
            task.AssignedToId = request.AssignedToId;
            task.IsMilestone = request.IsMilestone;
            task.AttachmentUrls = request.AttachmentUrls;

            return await _taskRepository.UpdateAsync(task);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return false;

            // Check if user is a member of the project or the owner
            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
                return false;

            bool isMember = project.OwnerId == userId || project.ProjectMembers.Any(pm => pm.UserId == userId);
            if (!isMember)
                return false;

            return await _taskRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateStatusAsync(int id, UpdateTaskStatusRequest request, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return false;

            // Check if user is a member of the project or the owner
            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null)
                return false;

            bool isMember = project.OwnerId == userId || project.ProjectMembers.Any(pm => pm.UserId == userId);
            if (!isMember)
                return false;

            // If the task is assigned to someone, only that person or the project owner can update the status
            if (task.AssignedToId.HasValue && task.AssignedToId.Value != userId && project.OwnerId != userId)
                return false;

            task.Status = request.Status;
            
            // If the task is completed, set the completed date
            if (request.Status == "Completed")
                task.CompletedDate = DateTime.UtcNow;
            else
                task.CompletedDate = null;

            var success = await _taskRepository.UpdateAsync(task);

            // Create notification for project owner if task is completed
            if (success && request.Status == "Completed" && project.OwnerId != userId)
            {
                var notification = new SRPM.Data.Entities.Notification
                {
                    Title = "Task Completed",
                    Message = $"The task '{task.Title}' has been marked as completed.",
                    Type = "Success",
                    RelatedEntityType = "Task",
                    RelatedEntityId = task.Id,
                    UserId = project.OwnerId
                };

                await _notificationRepository.CreateAsync(notification);
            }

            return success;
        }

        private TaskDto MapToDto(TaskEntity task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                ProjectId = task.ProjectId,
                ProjectTitle = task.Project?.Title,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo?.Name,
                IsMilestone = task.IsMilestone,
                AttachmentUrls = task.AttachmentUrls,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
