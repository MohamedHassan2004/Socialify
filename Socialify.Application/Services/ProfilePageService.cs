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

namespace Socialify.Application.Services
{
    public class ProfilePageService : IProfilePageService
    {
        private readonly IMapper _mapper;
        private readonly IProfileService _profileService;

        public ProfilePageService(IMapper mapper,IProfileService profileService)
        {
            _mapper = mapper;
            _profileService = profileService;
        }
        public async Task<Result<ProfilePageDto>> GetProfilePageAsync(string targetUserId, string currentUserId)
        {
            var profileResult = await _profileService.GetUserProfileAsync(targetUserId, currentUserId);
            if (!profileResult.IsSuccess)
                return Result<ProfilePageDto>.Failure(profileResult.ErrorMessage);

            var profileInfo = profileResult.Data;

            //var postsResult = await _postService.GetPostsForUserAsync(userId);
            //var friendsResult = await _friendService.GetFriendsForUserAsync(userId);

            var dto = new ProfilePageDto
            {
                User = profileInfo
                //Posts = postsResult.IsSuccess ? postsResult.Data : new List<PostDto>(),
                //Friends = friendsResult.IsSuccess ? friendsResult.Data : new List<FriendDto>()
            };

            return Result<ProfilePageDto>.Success(dto);
        }
    }
}
