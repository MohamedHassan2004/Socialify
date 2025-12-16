using Microsoft.EntityFrameworkCore;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Repository
{
    public class FriendshipRepository : Repository<Friendship>, IFriendshipRepository
    {
        private readonly SocialifyDbContext _context;

        public FriendshipRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProfileBasicInfoDto>> GetFriendshipAsync(
            string userId, 
            string currentUserId, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Friendships
                .Where(f => f.UserId == userId)
                .Select(f => new ProfileBasicInfoDto
                {
                    Id = f.Friend.Id,
                    FullName = f.Friend.FullName,
                    ProfilePicUrl = f.Friend.ProfilePicUrl,
                    Bio = f.Friend.Bio,
                    RelationshipStatus = 
                        f.Friend.Id == currentUserId ? RelationshipStatus.Self :
                        f.Friend.Friendships.Any(fs => fs.UserId == currentUserId) ? RelationshipStatus.Friend :
                        f.Friend.SentFriendRequests.Any(fr => fr.ReceiverId == currentUserId) ? RelationshipStatus.ReceivedRequest :
                        f.Friend.ReceivedFriendRequests.Any(fr => fr.SenderId == currentUserId) ? RelationshipStatus.SentRequest :
                        RelationshipStatus.None
                })
                .AsNoTracking();

            return await query.OrderByDescending(q => q.FullName).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<Friendship>> GetMyFriendshipAsync(string currentUserId, int pageNumber, int pageSize)
        {
            var query = _context.Friendships
                .Where(f => f.UserId == currentUserId)
                .Include(f => f.Friend)
                .AsNoTracking();

            return await query.OrderByDescending(q => q.Friend.FullName).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Friendship>> GetFriendshipByUserIdAsync(string userId)
        {
            return await _context.Friendships
                .Where(f => f.UserId == userId || f.FriendId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
