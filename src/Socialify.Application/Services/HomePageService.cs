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
        private readonly INotificationService _notificationService;
        private readonly IFriendRequestService _friendRequestService;

        public HomePageService(
            ILogger<HomePageService> logger,
            IProfileService profileService,
            IPostService postService,
            INotificationService notificationService,
            IFriendRequestService friendRequestService)
        {
            _logger = logger;
            _profileService = profileService;
            _postService = postService;
            _notificationService = notificationService;
            _friendRequestService = friendRequestService;
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
                var notificationsCount = await _notificationService.GetUnreadNotificationsCountAsync(currentUserId);
                var friendRequestsCount = await _friendRequestService.GetIncomingFriendRequestsCountAsync(currentUserId);

                if (!posts.IsSuccess || !userInfo.IsSuccess || !notificationsCount.IsSuccess || !friendRequestsCount.IsSuccess)
                    return Result<HomePageDto>.Failure("Failed to fetch posts or user info.");

                var dto = new HomePageDto
                {
                    User = userInfo.Data,
                    Posts = posts.Data,
                    NotificationsCount = notificationsCount.Data,
                    FriendRequestsCount = friendRequestsCount.Data

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
