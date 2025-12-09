using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Home;
using Socialify.Application.DTOs.Post;
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
    public class HomePageService : IHomePageService
    {
        private readonly ILogger<HomePageService> _logger;
        private readonly IProfileService _profileService;
        private readonly IPostService _postService;
        //private readonly IFriendService _friendService;

        public HomePageService(ILogger<HomePageService> logger, IProfileService profileService, IPostService postService)// IFriendService friendService)
        {
            _logger = logger;
            _profileService = profileService;
            _postService = postService;
            //_friendService = friendService;
        }

        public async Task<Result<HomePageDto>> GetHomePageAsync(string currentUserId, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(currentUserId))
                    return Result<HomePageDto>.Failure("Invalid user ID.");

                var paramsDto = new PaginationParamsDto
                {
                    PageNumber = 1,
                    PageSize = pageSize,
                    CurrentUserId = currentUserId
                };

                var userInfo = await _profileService.GetProfileBasicInfoAsync(currentUserId);
                var posts = await _postService.GetRelevantFeedsAsync(paramsDto);
                //var peopleYouMayKnow = await _friendService.GetPeopleYouMayKnowAsync(currentUserId);

                if (!posts.IsSuccess || !userInfo.IsSuccess)
                    return Result<HomePageDto>.Failure("Failed to fetch posts or user info.");

                var dto = new HomePageDto
                {
                    User = userInfo.Data,
                    Posts = posts.Data
                    //PeopleYouMayKnow = peopleYouMayKnow.Data ?? new List<UserDto>()
                };

                return Result<HomePageDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching home page data for user {UserId}", currentUserId);
                return Result<HomePageDto>.Failure("An error occurred while fetching the home page data.");
            }
        }
    }

}
