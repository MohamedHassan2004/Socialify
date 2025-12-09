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
    public class LikeRepository : Repository<Like> , ILikeRepository
    {
        private readonly SocialifyDbContext _context;

        public LikeRepository(SocialifyDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<PagedResult<Like>> GetLikesOnPostAsync(int postId, string currentUserId, int pageNumber, int pageSize)
        {
            return await _context.Likes
                .Where(l => l.PostId == postId)
                .Include(l => l.User)
                    .ThenInclude(u => u.Friendships)
                .Include(l => l.User)
                    .ThenInclude(u => u.SentFriendRequests)
                .Include(l => l.User)
                    .ThenInclude(u => u.ReceivedFriendRequests)
                .OrderByDescending(l => l.LikedAt)
                .AsSplitQuery()
                .AsNoTracking()
                .ToPagedResultAsync(pageNumber, pageSize);
        }
    }
}
