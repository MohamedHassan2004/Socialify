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
    public interface ILikeService
    {
        Task<Result> ToggleLikeAsync(string userId, int postId);
        Task<Result<PagedResult<ProfileBasicInfoDto>>> GetLikesOnPostAsync(int postId, PaginationParamsDto paramsDto);
    }
}
