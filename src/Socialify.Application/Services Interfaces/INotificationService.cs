using Socialify.Application.DTOs.Notification;
using Socialify.Domain.Common;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface INotificationService
    {
        Task<Result> SendNotificationAsync(string currentUserId, NotificationType type, string userId, int? postId = null);
        Task<Result> DeleteNotificationAsync(string currentUserId, NotificationType type, string userId, int? postId = null);
        Task<Result> MakeAllAsReadAsync(string userId);
        Task<Result<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(string userId);
        Task<Result<int>> GetUnreadNotificationsCountAsync(string userId);
    }
}
