using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services
{
    public class ProfilePageService : IProfilePageService
    {
        private readonly ILogger<ProfilePageService> _logger;
        private readonly IProfileService _profileService;
        private readonly IPostService _postService;
        private readonly IFriendshipService _friendshipService;

        public ProfilePageService(ILogger<ProfilePageService> logger, IProfileService profileService, IPostService postService, IFriendshipService friendshipService)
        {
            _logger = logger;
            _profileService = profileService;
            _postService = postService;
            _friendshipService = friendshipService;
        }

        public async Task<Result<ProfilePageDto>> GetProfilePageAsync(string targetUserId, string currentUserId, int pageSize)
        {
            var paramDto = new PaginationParamsDto
            {
                PageNumber = 1,
                PageSize = pageSize,
                CurrentUserId = currentUserId
            };

            var profileInfoResult = await _profileService.GetUserProfileAsync(targetUserId, currentUserId);
            var postsResult = await _postService.GetPostsByUserIdAsync(targetUserId, paramDto);
            var friendsResult = (targetUserId == currentUserId)?
                await _friendshipService.GetMyFriendshipsAsync(paramDto):
                await _friendshipService.GetFriendshipsByUserIdAsync(targetUserId, paramDto);
                
            if (!profileInfoResult.IsSuccess || !postsResult.IsSuccess || !friendsResult.IsSuccess)
            {
                _logger.LogError("Failed to get profile info for user {UserId}: {Error}", targetUserId, profileInfoResult.ErrorMessage);
                return Result<ProfilePageDto>.Failure("Failed to fetch user profile");
            }

            var profilePageDto = new ProfilePageDto
            {
                ProfileInfo = profileInfoResult.Data!,
                Posts = postsResult.Data!,
                Friends = friendsResult.Data!
            };

            return Result<ProfilePageDto>.Success(profilePageDto);
        }
    }
}
