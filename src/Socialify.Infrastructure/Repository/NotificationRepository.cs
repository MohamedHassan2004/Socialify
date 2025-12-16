using Microsoft.EntityFrameworkCore;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Repository
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly SocialifyDbContext _context;
        public NotificationRepository(SocialifyDbContext context): base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetUnReadNotificationsByUserId(string userId)
        {
            return await _context.Notifications
                .Where(n => (n.ReceiverUserId == userId) && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserId(string userId)
        {
            return await _context.Notifications
                .Where(n => n.ReceiverUserId == userId)
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .ToListAsync();
        }

        public async Task<Notification?> GetNotificationWithUserAsync(int notificationId)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == notificationId);
        }

        public async Task<int> GetUnreadNotificationsCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.ReceiverUserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<Notification> GetNotificationAsync(string userId, NotificationType type, string receiverUserId, int? postId)
        {
            var notification = await _context.Notifications.FirstAsync(n =>
                n.UserId == userId &&
                n.ReceiverUserId == receiverUserId &&
                n.NotificationType == type &&
                n.PostId == postId);
            return notification;
        }
    }
}
