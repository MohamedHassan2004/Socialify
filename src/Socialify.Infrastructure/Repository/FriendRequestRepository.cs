using Microsoft.EntityFrameworkCore;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Repository
{
    public class FriendRequestRepository : Repository<FriendRequest>, IFriendRequestRepository
    {
        private readonly SocialifyDbContext _context;

        public FriendRequestRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<FriendRequest?> GetFriendRequestAsync(string senderId, string receiverId)
        {
            return await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
        }

        public async Task<IEnumerable<FriendRequest>> GetIncomingRequestsAsync(string userId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId)
                .Include(fr => fr.Sender)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<FriendRequest>> GetOutgoingRequestsAsync(string userId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.SenderId == userId)
                .Include(fr => fr.Receiver)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
