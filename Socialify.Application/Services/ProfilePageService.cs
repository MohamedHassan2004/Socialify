using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
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

        public ProfilePageService(ILogger<ProfilePageService> logger,IProfileService profileService, IPostService postService)
        {
            _logger = logger;
            _profileService = profileService;
            _postService = postService;
        }

        public async Task<Result<ProfilePageDto>> GetProfilePageAsync(string targetUserId, string currentUserId, int pageSize)
        {
            var profileInfoResult = await _profileService.GetUserProfileAsync(targetUserId, currentUserId);
            var postsResult = await _postService.GetPostsByUserIdAsync(targetUserId, currentUserId, 1, pageSize);

            if (!profileInfoResult.IsSuccess || !postsResult.IsSuccess)
            {
                _logger.LogError("Failed to get profile info for user {UserId}: {Error}", targetUserId, profileInfoResult.ErrorMessage);
                return Result<ProfilePageDto>.Failure("Failed to fetch user profile");
            }

            var profilePageDto = new ProfilePageDto
            {
                ProfileInfo = profileInfoResult.Data!,
                Posts = postsResult.Data!
            };

            return Result<ProfilePageDto>.Success(profilePageDto);
        }
    }
}
