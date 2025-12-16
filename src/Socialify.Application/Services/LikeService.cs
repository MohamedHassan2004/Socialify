using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Mappers;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILogger<LikeService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public LikeService(ILogger<LikeService> logger, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result> ToggleLikeAsync(string userId, int postId)
        {
            try
            {
                var existingLike = await _unitOfWork.Likes.SingleOrDefaultAsync(like => like.UserId == userId && like.PostId == postId);
                var post = await _unitOfWork.Posts.GetByIdAsync(postId);

                if (existingLike != null)
                {
                    _unitOfWork.Likes.Remove(existingLike);
                    post?.DecrementLikesCount();
                    await _notificationService.DeleteNotificationAsync(userId, NotificationType.Like, post!.UserId, postId);
                }
                else
                {
                    await _unitOfWork.Likes.AddAsync(new Like { UserId = userId, PostId = postId });
                    post?.IncrementLikes();
                    await _notificationService.SendNotificationAsync(userId, NotificationType.Like, post!.UserId, postId);
                }

                await _unitOfWork.SaveAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like for post {PostId} by user {UserId}", postId, userId);
                return Result.Failure($"An error occurred while toggling like the post");
            }
        }

        public async Task<Result<PagedResult<ProfileBasicInfoDto>>> GetLikesOnPostAsync(int postId, PaginationParamsDto paramsDto)
        {
            try
            {
                var result = await _unitOfWork.Likes.GetLikesOnPostAsync(postId, paramsDto.CurrentUserId, paramsDto.PageNumber, paramsDto.PageSize);
                if (result == null)
                {
                    return Result<PagedResult<ProfileBasicInfoDto>>.Failure("No likes found for the specified post.");
                }

                var profilesDto = result.Data.Select(like => like.User.ToProfileBasicInfoDto(paramsDto.CurrentUserId)).ToList();

                var pagedResult = new PagedResult<ProfileBasicInfoDto>
                {
                    Data = profilesDto,
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                };

                return Result<PagedResult<ProfileBasicInfoDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving likes for post {PostId}", postId);
                return Result<PagedResult<ProfileBasicInfoDto>>.Failure("An error occurred while retrieving likes.");
            }
        }
    }
}
