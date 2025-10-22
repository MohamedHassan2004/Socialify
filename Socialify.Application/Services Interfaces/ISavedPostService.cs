using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface ISavedPostService
    {
        Task<Result> ToggleSavePost(string userId, int postId);
        Task<Result<PagedResult<PostDto>>> GetSavedPostsAsync(PaginationParamsDto paramsDto);
    }
}
