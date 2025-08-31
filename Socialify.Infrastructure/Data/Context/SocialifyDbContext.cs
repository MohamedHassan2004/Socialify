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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }
    }
}