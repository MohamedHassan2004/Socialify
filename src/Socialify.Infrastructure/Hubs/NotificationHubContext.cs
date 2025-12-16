using Microsoft.AspNetCore.SignalR;
using Socialify.Application.DTOs.Notification;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Hubs
{
    public class NotificationHubContext : INotificationHubContext
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubContext(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUserAsync(string userId, NotificationDto notification)
        {
            await _hubContext.Clients.Group(userId)
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task RemoveNotificationFromUserAsync(string userId, int notificationId)
        {
            await _hubContext.Clients.Group(userId)
                .SendAsync("RemoveNotification", notificationId);
        }
    }
}
