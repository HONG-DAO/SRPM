using Microsoft.EntityFrameworkCore;
using SRPM.Data.Entities;
using TaskEntity = SRPM.Data.Entities.Task;

namespace SRPM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<FundingRequest> FundingRequests { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<ResearchTopic> ResearchTopics { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure UserRole entity
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Configure ProjectMember entity
            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMembers)
                .HasForeignKey(pm => pm.UserId);

            // Configure Task entity
            modelBuilder.Entity<TaskEntity>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId);

            modelBuilder.Entity<TaskEntity>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure FundingRequest entity
            modelBuilder.Entity<FundingRequest>()
                .HasOne(fr => fr.Project)
                .WithMany(p => p.FundingRequests)
                .HasForeignKey(fr => fr.ProjectId);

            modelBuilder.Entity<FundingRequest>()
                .HasOne(fr => fr.RequestedBy)
                .WithMany(u => u.FundingRequests)
                .HasForeignKey(fr => fr.RequestedById);

            // Configure Evaluation entity
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Project)
                .WithMany(p => p.Evaluations)
                .HasForeignKey(e => e.ProjectId);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.EvaluatedBy)
                .WithMany(u => u.Evaluations)
                .HasForeignKey(e => e.EvaluatedById);

            // Configure ResearchTopic entity
            modelBuilder.Entity<ResearchTopic>()
                .HasOne(rt => rt.CreatedBy)
                .WithMany(u => u.ResearchTopics)
                .HasForeignKey(rt => rt.CreatedById);

            // Configure Notification entity
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Administrator" },
                new Role { Id = 2, Name = "Staff", Description = "Staff Member" },
                new Role { Id = 3, Name = "PrincipalInvestigator", Description = "Principal Investigator" },
                new Role { Id = 4, Name = "Researcher", Description = "Researcher Member" },
                new Role { Id = 5, Name = "HostInstitution", Description = "Host Institution" },
                new Role { Id = 6, Name = "AppraisalCouncil", Description = "Appraisal Council Member" }
            );
        }
    }
}
