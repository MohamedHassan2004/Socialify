using Microsoft.EntityFrameworkCore;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;

namespace Socialify.Infrastructure.Repository
{
    public class SharedPostRepository : Repository<SharedPost>, ISharedPostRepository
    {
        private readonly SocialifyDbContext _context;

        public SharedPostRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SharedPost?> GetByOriginalAndSharedPostIdsAsync(int originalPostId, int sharedPostId)
        {
            return await _context.SharedPosts
                .FirstOrDefaultAsync(sp => sp.OriginalPostId == originalPostId && sp.SharedPostId == sharedPostId);
        }

        public async Task<bool> UserHasSharedPostAsync(int originalPostId, string userId)
        {
            return await _context.SharedPosts
                .AnyAsync(sp => sp.OriginalPostId == originalPostId && sp.SharedByUserId == userId);
        }
    }
}
