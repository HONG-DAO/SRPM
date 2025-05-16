using SRPM.API.Models;
using SRPM.Data.Entities;
using SRPM.Data.Repositories;
using Task = System.Threading.Tasks.Task;

namespace SRPM.API.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public EvaluationService(
            IEvaluationRepository evaluationRepository,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            INotificationRepository notificationRepository)
        {
            _evaluationRepository = evaluationRepository;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<EvaluationDto?> GetByIdAsync(int id)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(id);
            if (evaluation == null)
                return null;

            return MapToDto(evaluation);
        }

        public async Task<IEnumerable<EvaluationDto>> GetAllAsync()
        {
            var evaluations = await _evaluationRepository.GetAllAsync();
            return evaluations.Select(MapToDto);
        }

        public async Task<IEnumerable<EvaluationDto>> GetByProjectIdAsync(int projectId)
        {
            var evaluations = await _evaluationRepository.GetByProjectIdAsync(projectId);
            return evaluations.Select(MapToDto);
        }

        public async Task<IEnumerable<EvaluationDto>> GetByTaskIdAsync(int taskId)
        {
            var evaluations = await _evaluationRepository.GetByTaskIdAsync(taskId);
            return evaluations.Select(MapToDto);
        }

        public async Task<IEnumerable<EvaluationDto>> GetByEvaluatedByIdAsync(int userId)
        {
            var evaluations = await _evaluationRepository.GetByEvaluatedByIdAsync(userId);
            return evaluations.Select(MapToDto);
        }

        public async Task<IEnumerable<EvaluationDto>> GetByTypeAsync(string type)
        {
            var evaluations = await _evaluationRepository.GetByTypeAsync(type);
            return evaluations.Select(MapToDto);
        }

        public async Task<EvaluationDto?> CreateAsync(CreateEvaluationRequest request, int userId)
        {
            // Check if user has AppraisalCouncil role
            var roles = await _userRepository.GetUserRolesAsync(userId);
            if (!roles.Any(r => r.Name == "AppraisalCouncil" || r.Name == "Admin"))
                return null;

            // Check if project exists
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null)
                return null;

            // Check if task exists if TaskId is provided
            if (request.TaskId.HasValue)
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId.Value);
                if (task == null || task.ProjectId != request.ProjectId)
                    return null;
            }

            var evaluation = new Evaluation
            {
                Type = request.Type,
                Content = request.Content,
                Score = request.Score,
                Feedback = request.Feedback,
                ProjectId = request.ProjectId,
                TaskId = request.TaskId,
                EvaluatedById = userId
            };

            var success = await _evaluationRepository.CreateAsync(evaluation);
            if (!success)
                return null;

            // Create notification for project owner
            var notification = new Notification
            {
                Title = "New Evaluation",
                Message = $"A new {evaluation.Type} evaluation has been submitted for your project '{project.Title}'.",
                Type = "Info",
                RelatedEntityType = "Evaluation",
                RelatedEntityId = evaluation.Id,
                UserId = project.OwnerId
            };

            await _notificationRepository.CreateAsync(notification);

            // If it's a task evaluation, notify the assigned user
            if (request.TaskId.HasValue)
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId.Value);
                if (task != null && task.AssignedToId.HasValue && task.AssignedToId.Value != project.OwnerId)
                {
                    var taskNotification = new Notification
                    {
                        Title = "Task Evaluation",
                        Message = $"Your task '{task.Title}' has been evaluated.",
                        Type = "Info",
                        RelatedEntityType = "Evaluation",
                        RelatedEntityId = evaluation.Id,
                        UserId = task.AssignedToId.Value
                    };

                    await _notificationRepository.CreateAsync(taskNotification);
                }
            }

            return MapToDto(evaluation);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEvaluationRequest request, int userId)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(id);
            if (evaluation == null)
                return false;

            // Only the evaluator can update the evaluation
            if (evaluation.EvaluatedById != userId)
            {
                // Or an admin
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            evaluation.Content = request.Content;
            evaluation.Score = request.Score;
            evaluation.Feedback = request.Feedback;

            return await _evaluationRepository.UpdateAsync(evaluation);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(id);
            if (evaluation == null)
                return false;

            // Only the evaluator can delete the evaluation
            if (evaluation.EvaluatedById != userId)
            {
                // Or an admin
                var roles = await _userRepository.GetUserRolesAsync(userId);
                if (!roles.Any(r => r.Name == "Admin"))
                    return false;
            }

            return await _evaluationRepository.DeleteAsync(id);
        }

        private EvaluationDto MapToDto(Evaluation evaluation)
        {
            return new EvaluationDto
            {
                Id = evaluation.Id,
                Type = evaluation.Type,
                Content = evaluation.Content,
                Score = evaluation.Score,
                Feedback = evaluation.Feedback,
                ProjectId = evaluation.ProjectId,
                ProjectTitle = evaluation.Project?.Title,
                TaskId = evaluation.TaskId,
                TaskTitle = evaluation.Task?.Title,
                EvaluatedById = evaluation.EvaluatedById,
                EvaluatedByName = evaluation.EvaluatedBy?.Name,
                CreatedAt = evaluation.CreatedAt,
                UpdatedAt = evaluation.UpdatedAt
            };
        }
    }
}
