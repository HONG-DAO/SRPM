using System.ComponentModel.DataAnnotations;

namespace SRPM.Data.Entities
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Status { get; set; } // NotStarted, InProgress, Completed, Delayed

        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public bool IsMilestone { get; set; }
        public string? AttachmentUrls { get; set; } // JSON array of URLs

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
