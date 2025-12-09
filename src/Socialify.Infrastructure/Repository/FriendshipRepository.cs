using Microsoft.EntityFrameworkCore;
using Socialify.Application.DTOs.Common;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Entities;
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

        // to optimize
        public async Task<PagedResult<Friendship>> GetFriendshipAsync(string userId, int pageNumber, int pageSize)
        {
            var query = _context.Friendships
                .Where(f => f.UserId == userId)
                .Include(f => f.Friend)
                    .ThenInclude(friend => friend.SentFriendRequests)
                .Include(f => f.Friend)
                    .ThenInclude(friend => friend.ReceivedFriendRequests)
                .AsSplitQuery()
                .AsNoTracking();

            return await query.OrderByDescending(q => q.Friend.FullName).ToPagedResultAsync(pageNumber, pageSize);
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
                .Where(f => f.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetFriendshipsForUsersAsync(List<string> userIds)
        {
            return await _context.Friendships
                .Where(f => userIds.Contains(f.UserId))
                .AsNoTracking()
                .ToListAsync();
        }

        //public async Task<IEnumerable<ApplicationUser>> GetPeopleYouMayKnow(string currentUserId)
        //{
        //    var friendsOfFriends = await _context.Friendships
        //        .Where(f => f.UserId == currentUserId)
        //        .SelectMany(f => _context.Friendships
        //            .Where(fof => fof.UserId == f.FriendId && fof.FriendId != currentUserId)
        //            .Select(fof => fof.Friend))
        //        .Distinct()
        //        .ToListAsync();

        //    var friendRequests = await _context.FriendRequests
        //        .Where(fr => fr.SenderId == currentUserId || fr.ReceiverId == currentUserId)
        //        .ToListAsync();

        //    var peopleYouMayKnow = friendsOfFriends.Where(f => friendRequests.(f.Id)).ToList();

        //    return peopleYouMayKnow;
        //}
    }
}
