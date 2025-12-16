using Humanizer;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Notification;
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
    public class NotificationService  : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationHubContext _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, INotificationHubContext hubContext, ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        private NotificationDto MapNotificaton(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                UserName = notification.User.FullName,
                UserProfilePicUrl = notification.User.ProfilePicUrl,
                Message = GetNotificationMessage(notification),
                Link = GetNotificationLink(notification),
                IsRead = notification.IsRead,
                TimeAgo = notification.CreatedAt.Humanize(false)
            };
        }

        private string GetNotificationLink(Notification notification) =>
            notification.NotificationType switch
            {
                NotificationType.Like
                or NotificationType.Comment
                or NotificationType.Share
                    => $"/Posts/GetPostWithComments?postId={notification.PostId}",

                NotificationType.FriendRequest
                    => "/Friendship/GetIncomingFriendRequests",

                NotificationType.AcceptedFriendRequest
                    => $"/Profile/{notification.UserId}",

                _ => throw new ArgumentOutOfRangeException(
                        nameof(notification.NotificationType),
                        $"Unhandled notification type: {notification.NotificationType}")
            };

        private string GetNotificationMessage(Notification notification)
        {
            return notification.NotificationType switch
            {
                NotificationType.Like => "liked your post.",
                NotificationType.Comment => "commented on your post.",
                NotificationType.Share => "shared your post.",
                NotificationType.FriendRequest => "sent you a friend request.",
                NotificationType.AcceptedFriendRequest => "accepted your friend request.",
                _ => "performed an action."
            };
        }

        public async Task<Result> MakeAllAsReadAsync(string userId)
        {
            try
            {
                var notifications = await _notificationRepository.GetUnReadNotificationsByUserId(userId);
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    _notificationRepository.Update(notification);
                }
                await _notificationRepository.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notifications as read for user {UserId}", userId);
                return Result.Failure($"An error occurred while marking notifications as read");
            }
        }

        public async Task<Result<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(string userId)
        {
            try
            {
                var notifications = await _notificationRepository.GetNotificationsByUserId(userId);
                var notificationDto = notifications.Select(n => MapNotificaton(n)).ToList();

                var readingResult = await MakeAllAsReadAsync(userId);
                if (!readingResult.IsSuccess)
                {
                    _logger.LogWarning("Failed Making Notifications as read of User {UserId}",userId);
                }

                _logger.LogInformation("Fetched Notifications of User {UserId} Successfully", userId);
                return Result<IEnumerable<NotificationDto>>.Success(notificationDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
                return Result<IEnumerable<NotificationDto>>.Failure($"An error occurred while retrieving notifications for user {userId}");
            }
        }

        public async Task<Result<int>> GetUnreadNotificationsCountAsync(string userId)
        {
            try
            {
                var notificationsCount = await _notificationRepository.GetUnreadNotificationsCountAsync(userId);
                _logger.LogInformation("Fetched Notifications Count of User {UserId} Successfully", userId);
                return Result<int>.Success(notificationsCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications count for user {UserId}", userId);
                return Result<int>.Failure($"An error occurred while retrieving notifications count for user {userId}");
            }
        }

        public async Task<Result> SendNotificationAsync(string currentUserId, NotificationType type, string userId, int? postId = null)
        {
            try
            {
                if (userId == currentUserId)
                {
                    return Result.Success();
                }

                var notification = new Notification
                {
                    ReceiverUserId = userId,
                    UserId = currentUserId,
                    NotificationType = type,
                    PostId = postId,
                };

                await _notificationRepository.AddAsync(notification);
                await _notificationRepository.SaveChangesAsync();

                var notificationWithUser = await _notificationRepository.GetNotificationWithUserAsync(notification.Id);
                if (notificationWithUser != null)
                {
                    var notificationDto = MapNotificaton(notificationWithUser);

                    await _hubContext.SendNotificationToUserAsync(userId, notificationDto);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
                return Result.Failure($"An error occurred while sending notification to user {userId}");
            }
        }

        public async Task<Result> DeleteNotificationAsync(string currentUserId, NotificationType type, string userId, int? postId = null)
        {
            try
            {
                var notification = await _notificationRepository.GetNotificationAsync(currentUserId, type, userId, postId);
                var notificationId = notification.Id;
                _notificationRepository.Remove(notification);
                var isDeleted = await _notificationRepository.SaveChangesAsync() > 0;
                if (isDeleted)
                {
                    await _hubContext.RemoveNotificationFromUserAsync(userId, notificationId);
                    return Result.Success();
                }
                else
                {
                    return Result.Failure("Failed to delete notification.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification for user {UserId}", userId);
                return Result.Failure($"An error occurred while deleting notification for user {userId}");
            }
        }
    }
}

