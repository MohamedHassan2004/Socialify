using MediatR;
using Microsoft.Extensions.Logging;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Entities;
using Socialify.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.EventHandlers
{
    public class DeleteUserPosts : INotificationHandler<UserDeletingEvent>
    {
        private readonly IFileManager _fileManager;
        private readonly IPostRepository _postRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteUserPosts> _logger;

        public DeleteUserPosts(IFileManager fileManager, IPostRepository postRepository,IMediator mediator, ILogger<DeleteUserPosts> logger)
        {
            _fileManager = fileManager;
            _postRepository = postRepository;
            _mediator = mediator;
            _logger = logger;
        }
        public async Task Handle(UserDeletingEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var postsWithMedia = await _postRepository.FindAsync(p => p.UserId == notification.UserId && p.MediaUrl != null);
                var mediaUrls = postsWithMedia
                    .Where(p => !string.IsNullOrEmpty(p.MediaUrl))
                    .Select(p => p.MediaUrl!)
                    .ToArray();

                foreach (var url in mediaUrls)
                {
                    _fileManager.DeleteFile(url);
                }

                var userPosts = await _postRepository.FindAsync(p => p.UserId == notification.UserId);
                foreach (var post in userPosts)
                {
                    await _mediator.Publish(new OriginalPostDeletingEvent(post.Id));
                    _postRepository.Remove(post);
                }
                await _postRepository.SaveChangesAsync();

                _logger.LogInformation("Deleted {Count} posts and {Files} media files for user {UserId}", userPosts.Count(), mediaUrls.Length, notification.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting posts for user {UserId}", notification.UserId);
                throw;
            }
        }
    }
}
