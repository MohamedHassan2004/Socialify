using AutoMapper;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Socialify.Application.Services
{
    public class ProfilePageService : IProfilePageService
    {
        private readonly IMapper _mapper;
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfilePageService> _logger;

        public ProfilePageService(
            IMapper mapper,
            IProfileService profileService,
            ILogger<ProfilePageService> logger)
        {
            _mapper = mapper;
            _profileService = profileService;
            _logger = logger;
        }

        public async Task<Result<ProfilePageDto>> GetProfilePageAsync(string targetUserId, string currentUserId)
        {
            try
            {
                // Single optimized call that loads user with posts
                var profileResult = await _profileService.GetUserProfileAsync(targetUserId, currentUserId);
                if (!profileResult.IsSuccess)
                {
                    _logger.LogError("Failed to load profile for user {TargetUserId}: {ErrorMessage}", 
                        targetUserId, profileResult.ErrorMessage);
                    return Result<ProfilePageDto>.Failure(profileResult.ErrorMessage);
                }

                var profileInfo = profileResult.Data;

                // TODO: Implement these when Post and Friend services are ready
                //var postsResult = await _postService.GetPostsForUserAsync(targetUserId);
                //var friendsResult = await _friendService.GetFriendsForUserAsync(targetUserId);

                var dto = new ProfilePageDto
                {
                    User = profileInfo
                    //Posts = postsResult.IsSuccess ? postsResult.Data : new List<PostDto>(),
                    //Friends = friendsResult.IsSuccess ? friendsResult.Data : new List<ProfileBasicInfoDto>()
                };

                _logger.LogInformation("Successfully loaded profile page for user {TargetUserId}", targetUserId);
                return Result<ProfilePageDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error loading profile page for user {TargetUserId}", targetUserId);
                return Result<ProfilePageDto>.Failure("An error occurred while fetching the profile page data.");
            }
        }
    }
}
