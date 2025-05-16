using System.ComponentModel.DataAnnotations;
using Task = SRPM.Data.Entities.Task;

namespace SRPM.Data.Entities
{
    public class Evaluation
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } // Project, Milestone, Proposal

        [Required]
        public string Content { get; set; }

        public int? Score { get; set; }
        public string? Feedback { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? TaskId { get; set; } // For milestone evaluations
        public Task? Task { get; set; }

        public int EvaluatedById { get; set; }
        public User EvaluatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
