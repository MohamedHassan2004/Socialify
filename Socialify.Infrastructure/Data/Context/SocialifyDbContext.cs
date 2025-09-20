using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Socialify.Domain.Entities;

namespace Socialify.Infrastructure.Data.Context
{
    public class SocialifyDbContext : IdentityDbContext<ApplicationUser>
    {
        public SocialifyDbContext(DbContextOptions<SocialifyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Post relationship
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for better query performance
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.UserId)
                .HasDatabaseName("IX_Posts_UserId");

            modelBuilder.Entity<Post>()
                .HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Posts_CreatedAt");

            // Configure ApplicationUser for better queries
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => new { u.FirstName, u.LastName })
                .HasDatabaseName("IX_Users_FirstName_LastName");

            // Configure query splitting for better performance
            modelBuilder.Entity<ApplicationUser>()
                .Navigation(u => u.Posts)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }

        // Enable query splitting for better performance
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            // Enable query splitting for better performance
            optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    }
}