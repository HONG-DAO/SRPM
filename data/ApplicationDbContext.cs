using Microsoft.EntityFrameworkCore;
using SRPM.Backend.Models;

namespace SRPM.Backend.Data {
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<FundingRequest> FundingRequests { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
