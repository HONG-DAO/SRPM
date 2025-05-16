namespace SRPM.API.Models
{
    public class EvaluationDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int? Score { get; set; }
        public string? Feedback { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public int? TaskId { get; set; }
        public string? TaskTitle { get; set; }
        public int EvaluatedById { get; set; }
        public string? EvaluatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateEvaluationRequest
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public int? Score { get; set; }
        public string? Feedback { get; set; }
        public int ProjectId { get; set; }
        public int? TaskId { get; set; }
    }

    public class UpdateEvaluationRequest
    {
        public string Content { get; set; }
        public int? Score { get; set; }
        public string? Feedback { get; set; }
    }
}
