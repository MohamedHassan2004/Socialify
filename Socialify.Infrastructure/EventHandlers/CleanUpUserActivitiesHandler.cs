using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.EventHandlers
{
    
    public class CleanUpUserActivitiesHandler : INotificationHandler<UserDeletingEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CleanUpUserActivitiesHandler> _logger;

        public CleanUpUserActivitiesHandler(IUnitOfWork unitOfWork, ILogger<CleanUpUserActivitiesHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(UserDeletingEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var savedPosts = await _unitOfWork.SavedPosts.FindAsync(sp => sp.UserId == notification.UserId);
                foreach (var savedPost in savedPosts)
                {
                    _unitOfWork.SavedPosts.Remove(savedPost);
                }

                var likes = await _unitOfWork.Likes
                    .FindAsync(l => l.UserId == notification.UserId, include: q => q.Include(l => l.Post));
                foreach (var like in likes)
                {
                    _unitOfWork.Likes.Remove(like);
                }

                var comments = await _unitOfWork.Comments
                    .FindAsync(c => c.UserId == notification.UserId, include: q => q.Include(c => c.Post));
                foreach (var comment in comments)
                {
                    _unitOfWork.Comments.Remove(comment);
                }

                var likedPosts = likes.Select(l => l.Post).ToList();
                foreach (var post in likedPosts)
                {
                    post.DecrementLikesCount();
                }

                var commentedPosts = comments.Select(c => c.Post).ToList();
                foreach (var post in commentedPosts)
                {
                    post.DecrementCommentsCount();
                }

                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "Deleted {Likes} likes, {Comments} comments, and {Saved} saved posts for user {UserId}",
                    likes.Count(), comments.Count(), savedPosts.Count(), notification.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up activities for user with ID: {UserId}", notification.UserId);
                throw;
            }
        }
    }
}
