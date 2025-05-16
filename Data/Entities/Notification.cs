using System.ComponentModel.DataAnnotations;

namespace SRPM.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Message { get; set; }
        public string? Type { get; set; } // Info, Warning, Success, Error
        public string? RelatedEntityType { get; set; } // Project, Task, FundingRequest, etc.
        public int? RelatedEntityId { get; set; }

        public bool IsRead { get; set; } = false;

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
