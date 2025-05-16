using System.ComponentModel.DataAnnotations;

namespace SRPM.Data.Entities
{
    public class ResearchTopic
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public string? Requirements { get; set; }
        public decimal? AvailableFunding { get; set; }

        public bool IsActive { get; set; } = true;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
