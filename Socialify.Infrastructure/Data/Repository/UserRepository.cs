using Socialify.Application.Interfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Data.Repository
{
    public class UserRepository : Repository<ApplicationUser>
    {
        private readonly SocialifyDbContext _context;

        public UserRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
