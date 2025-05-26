using System.ComponentModel.DataAnnotations;
using Task = SRPM.Data.Entities.Task;

namespace SRPM.Data.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(255)]
        public string? AvatarUrl { get; set; }

        [MaxLength(255)]
        public string? BackgroundUrl { get; set; }

        [MaxLength(255)]
        public string? SocialLinks { get; set; }

        public string? GoogleId { get; set; }
        public string? VerificationCode { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
        public ICollection<FundingRequest> FundingRequests { get; set; } = new List<FundingRequest>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        public ICollection<ResearchTopic> ResearchTopics { get; set; } = new List<ResearchTopic>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    }
}
