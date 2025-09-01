using Socialify.Application.Interfaces;
using Socialify.Application.RepoInterfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Data.Repository
{
    public class ProfileRepository : Repository<ApplicationUser>, IProfileRepository
    {
        private readonly SocialifyDbContext _context;

        public ProfileRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<ApplicationUser> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
