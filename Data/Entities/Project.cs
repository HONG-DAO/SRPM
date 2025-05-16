using System.ComponentModel.DataAnnotations;

namespace SRPM.Data.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }

        [Required]
        public string Status { get; set; } // Draft, Submitted, Approved, InProgress, Completed, Rejected

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int OwnerId { get; set; } // Principal Investigator
        public User Owner { get; set; }

        public int? ResearchTopicId { get; set; }
        public ResearchTopic? ResearchTopic { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<FundingRequest> FundingRequests { get; set; } = new List<FundingRequest>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}
