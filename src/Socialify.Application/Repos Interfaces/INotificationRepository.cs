using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Repos_Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUnReadNotificationsByUserId(string userId);
        Task<IEnumerable<Notification>> GetNotificationsByUserId(string userId);
        Task<Notification?> GetNotificationWithUserAsync(int notificationId);
        Task<int> GetUnreadNotificationsCountAsync(string userId);
        Task<Notification> GetNotificationAsync(string userId, NotificationType type, string receiverUserId, int? postId);
    }
}
