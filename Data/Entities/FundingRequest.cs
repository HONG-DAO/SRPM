using System.ComponentModel.DataAnnotations;

namespace SRPM.Data.Entities
{
    public class FundingRequest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Approved, Rejected

        public string? Purpose { get; set; }
        public string? JustificationDocumentUrl { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int RequestedById { get; set; }
        public User RequestedBy { get; set; }

        public int? ApprovedById { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
