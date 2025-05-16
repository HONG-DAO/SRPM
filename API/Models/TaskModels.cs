namespace SRPM.API.Models
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public bool IsMilestone { get; set; }
        public string? AttachmentUrls { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTaskRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedToId { get; set; }
        public bool IsMilestone { get; set; }
        public string? AttachmentUrls { get; set; }
    }

    public class UpdateTaskRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public int? AssignedToId { get; set; }
        public bool IsMilestone { get; set; }
        public string? AttachmentUrls { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        public string Status { get; set; }
    }
}
