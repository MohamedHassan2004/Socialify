using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Events;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.EventHandlers
{
    
    public class CleanUpUserActivitiesHandler : INotificationHandler<UserDeletingEvent>
    {
        private readonly SocialifyDbContext _context;
        private readonly ILogger<CleanUpUserActivitiesHandler> _logger;

        public CleanUpUserActivitiesHandler(SocialifyDbContext context, ILogger<CleanUpUserActivitiesHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(UserDeletingEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // delete saved posts
                await _context.SavedPosts.Where(sp => sp.UserId == notification.UserId).ExecuteDeleteAsync(cancellationToken);

                // delete likes
                var likes = await _context.Likes
                    .Where(l => l.UserId == notification.UserId)
                    .Include(l => l.Post)
                    .ToListAsync();
                _context.Likes.RemoveRange(likes);

                var likedPosts = likes.Select(l => l.Post).ToList();
                foreach (var post in likedPosts)
                {
                    post.DecrementLikesCount();
                }

                // delete comments
                var comments = await _context.Comments
                    .Where(l => l.UserId == notification.UserId)
                    .Include(c => c.Post)
                    .ToListAsync();
                _context.Comments.RemoveRange(comments);

                var commentedPosts = comments.Select(c => c.Post).ToList();
                foreach (var post in commentedPosts)
                {
                    post.DecrementCommentsCount();
                }

                // delete shared posts
                await _context.SharedPosts.Where(sp => sp.SharedByUserId == notification.UserId).ExecuteDeleteAsync(cancellationToken);


                // delete notifications
                await _context.Notifications.Where(n => n.ReceiverUserId == notification.UserId).ExecuteDeleteAsync(cancellationToken);


                var rowsAffected = await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted {rowsAffected} records for user {UserId}", rowsAffected, notification.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up activities for user with ID: {UserId}", notification.UserId);
                throw;
            }
        }
    }
}
