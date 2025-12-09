using Microsoft.EntityFrameworkCore;
using Socialify.Application.DTOs.Common;
using Socialify.Application.Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System.Linq.Expressions;

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

        public async Task<ApplicationUser?> GetUserProfileByIdAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Friendships)
                .Include(u => u.SentFriendRequests)
                .Include(u => u.ReceivedFriendRequests)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<PagedResult<ApplicationUser>> SearchUsersAsync(string searchTerm, int pageNumber, int pageSize)
        {
            return await _context.Users
                .Where(u => EF.Functions.Like(u.FullName, $"%{searchTerm}%"))
                .OrderBy(p => p.FirstName)
                .Include(u => u.Friendships)
                .Include(u => u.SentFriendRequests)
                .Include(u => u.ReceivedFriendRequests)
                .AsSplitQuery()
                .AsNoTracking()
                .ToPagedResultAsync(pageNumber, pageSize);
        }

    }
}
