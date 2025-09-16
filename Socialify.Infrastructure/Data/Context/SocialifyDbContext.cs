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

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasIndex(p => p.UserId);




        }
    }
}