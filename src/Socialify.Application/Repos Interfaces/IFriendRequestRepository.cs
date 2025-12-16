using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Repos_Interfaces
{
    public interface IFriendRequestRepository : IRepository<FriendRequest>
    {
        Task<IEnumerable<FriendRequest>> GetIncomingRequestsAsync(string userId);
        Task<IEnumerable<FriendRequest>> GetOutgoingRequestsAsync(string userId);
        Task<FriendRequest?> GetFriendRequestAsync(string senderId, string receiverId);
        Task<int> GetIncomingRequestsCountAsync(string userId);
    }
}
