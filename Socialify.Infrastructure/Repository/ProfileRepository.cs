using Microsoft.EntityFrameworkCore;
using Socialify.Application.Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Repository
{
    public class ProfileRepository : Repository<ApplicationUser>, IProfileRepository
    {
        private readonly SocialifyDbContext _context;

        public ProfileRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _context.FindAsync<ApplicationUser>(userId);
        }
    }
}
