using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Repos_Interfaces
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        Task<PagedResult<ProfileBasicInfoDto>> GetFriendshipAsync(string userId, string currentUserId, int pageNumber, int pageSize);
        Task<PagedResult<Friendship>> GetMyFriendshipAsync(string currentUserId, int pageNumber, int pageSize);
        Task<IEnumerable<Friendship>> GetFriendshipByUserIdAsync(string userId);
    }
}
