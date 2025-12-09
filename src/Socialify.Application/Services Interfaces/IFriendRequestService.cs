using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface IFriendRequestService
    {
        Task<Result> SendFriendRequestAsync(string senderId, string receiverId);
        Task<Result> RemoveFriendRequestAsync(string senderId, string receiverId);
        Task<Result> AcceptFriendRequestAsync(string senderId, string receiverId);
        Task<Result<IEnumerable<ProfileBasicInfoDto>>> GetOutgoingRequestsAsync(string userId);
        Task<Result<IEnumerable<ProfileBasicInfoDto>>> GetIncomingRequestsAsync(string userId);
    }
}
