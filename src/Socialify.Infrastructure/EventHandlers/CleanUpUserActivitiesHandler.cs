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
        private readonly ISharedPostRepository _sharedPostRepository;
        private readonly ILogger<CleanUpUserActivitiesHandler> _logger;

        public CleanUpUserActivitiesHandler(IUnitOfWork unitOfWork, ISharedPostRepository sharedPostRepository, ILogger<CleanUpUserActivitiesHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _sharedPostRepository = sharedPostRepository;
            _logger = logger;
        }

        public async Task Handle(UserDeletingEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // delete saved posts
                var savedPosts = await _unitOfWork.SavedPosts.FindAsync(sp => sp.UserId == notification.UserId);
                _unitOfWork.SavedPosts.RemoveRange(savedPosts);

                // delete likes
                var likes = await _unitOfWork.Likes
                    .FindAsync(l => l.UserId == notification.UserId, include: q => q.Include(l => l.Post));
                _unitOfWork.Likes.RemoveRange(likes);

                var likedPosts = likes.Select(l => l.Post).ToList();
                foreach (var post in likedPosts)
                {
                    post.DecrementLikesCount();
                }

                // delete comments
                var comments = await _unitOfWork.Comments
                    .FindAsync(c => c.UserId == notification.UserId, include: q => q.Include(c => c.Post));
                _unitOfWork.Comments.RemoveRange(comments);

                var commentedPosts = comments.Select(c => c.Post).ToList();
                foreach (var post in commentedPosts)
                {
                    post.DecrementCommentsCount();
                }

                // delete shared posts
                var sharedPosts = await _sharedPostRepository.FindAsync(sp => sp.SharedByUserId == notification.UserId);
                _sharedPostRepository.RemoveRange(sharedPosts);


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
