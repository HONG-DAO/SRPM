namespace SRPM.Data.Entities
{
    public class ProjectMember
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Role { get; set; } // PI, Researcher, etc.
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
