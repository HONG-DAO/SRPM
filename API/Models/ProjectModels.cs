namespace SRPM.API.Models
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int? ResearchTopicId { get; set; }
        public string? ResearchTopicTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProjectRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ResearchTopicId { get; set; }
    }

    public class UpdateProjectRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ResearchTopicId { get; set; }
    }

    public class AddProjectMemberRequest
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}
