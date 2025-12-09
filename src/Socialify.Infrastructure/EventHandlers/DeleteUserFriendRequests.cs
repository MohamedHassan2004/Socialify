using MediatR;
using Microsoft.Extensions.Logging;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.EventHandlers
{
    public class DeleteUserFriendRequests : INotificationHandler<UserDeletingEvent>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly ILogger<DeleteUserFriendRequests> _logger;

        public DeleteUserFriendRequests(IFriendshipRepository friendshipRepository, IFriendRequestRepository friendRequestRepository, ILogger<DeleteUserFriendRequests> logger)
        {
            _friendshipRepository = friendshipRepository;
            _friendRequestRepository = friendRequestRepository;
            _logger = logger;
        }

        public async Task Handle(UserDeletingEvent notification, CancellationToken cancellationToken)
        {
            var userId = notification.UserId;
            try
            {
                // Delete incoming friend requests
                var incomingRequests = await _friendRequestRepository.GetIncomingRequestsAsync(userId);
                _friendRequestRepository.RemoveRange(incomingRequests);

                // Delete outgoing friend requests
                var outgoingRequests = await _friendRequestRepository.GetOutgoingRequestsAsync(userId);
                _friendRequestRepository.RemoveRange(outgoingRequests);

                // Delete friendships
                var friendships = await _friendshipRepository.GetFriendshipByUserIdAsync(userId);
                _friendshipRepository.RemoveRange(friendships);

                await _friendRequestRepository.SaveChangesAsync();
                _logger.LogInformation("Deleted friend requests and friendships for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting friend requests and friendships for user with ID: {UserId}", userId);
                throw;
            }
        }
    }
}
