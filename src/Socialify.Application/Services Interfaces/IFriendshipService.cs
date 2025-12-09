using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface IFriendshipService
    {
        Task<Result> AddFriendshipAsync(string userId1, string userId2);
        Task<Result> RemoveFriendshipAsync(string userId1, string userId2);
        Task<Result<PagedResult<ProfileBasicInfoDto>>> GetMyFriendshipsAsync(PaginationParamsDto paramsDto);
        Task<Result<PagedResult<ProfileBasicInfoDto>>> GetFriendshipsByUserIdAsync(string userId, PaginationParamsDto paramsDto);
    }
}
