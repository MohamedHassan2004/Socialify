using Socialify.Application.DTOs.Notification;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface INotificationHubContext
    {
        Task SendNotificationToUserAsync(string userId, NotificationDto notification);
        Task RemoveNotificationFromUserAsync(string userId, int notificationId);
    }
}
