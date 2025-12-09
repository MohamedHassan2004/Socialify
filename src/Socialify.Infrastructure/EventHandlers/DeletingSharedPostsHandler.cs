using MediatR;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.EventHandlers
{
    public class DeletingSharedPostsHandler : INotificationHandler<OriginalPostDeletingEvent>
    {
        private readonly IPostRepository _postRepository;

        public DeletingSharedPostsHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task Handle(OriginalPostDeletingEvent notification, CancellationToken cancellationToken)
        {
            var postId = notification.PostId;
            var sharedPosts = await _postRepository.FindAsync(p => p.IsShared && p.OriginalPostId == postId);

            _postRepository.RemoveRange(sharedPosts);

            await _postRepository.SaveChangesAsync();
        }
    }
}
