using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Mappers;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Socialify.Application.Services
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly IFriendshipService _friendshipService;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<FriendRequestService> _logger;

        public FriendRequestService(
            ILogger<FriendRequestService> logger,
            IFriendRequestRepository friendRequestRepository,
            IFriendshipService friendshipService,
            IFriendshipRepository friendshipRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _friendRequestRepository = friendRequestRepository;
            _friendshipService = friendshipService;
            _friendshipRepository = friendshipRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<Result<int>> GetIncomingFriendRequestsCountAsync(string currentUserId)
        {
            try
            {
                var count = await _friendRequestRepository.GetIncomingRequestsCountAsync(currentUserId);
                _logger.LogInformation("Fetched Incoming Friend Requests Count for user {UserId} successfully", currentUserId);
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while Getting Incoming Requests Count");
                return Result<int>.Failure("Error occured while Getting Incoming Requests Count");
            }
        }

        public async Task<Result<IEnumerable<ProfileBasicInfoDto>>> GetIncomingRequestsAsync(string currentUserId)
        {
            try
            {
                var requests = await _friendRequestRepository.GetIncomingRequestsAsync(currentUserId);
                if(requests == null)
                {
                    return Result<IEnumerable<ProfileBasicInfoDto>>.Success(new List<ProfileBasicInfoDto>());
                }
                var requestDtos = requests.Select(r => r.Sender.ToProfileBasicInfoDto(currentUserId, RelationshipStatus.ReceivedRequest));
                _logger.LogInformation("Fetched Incoming Friend Requests for user {UserId} successfully", currentUserId);
                return Result<IEnumerable<ProfileBasicInfoDto>>.Success(requestDtos);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error occured while Getting Incoming Requests");
                return Result<IEnumerable<ProfileBasicInfoDto>>.Failure("Error occured while Getting Incoming Requests");
            }
        }

        public async Task<Result<IEnumerable<ProfileBasicInfoDto>>> GetOutgoingRequestsAsync(string currentUserId)
        {
            try
            {
                var requests = await _friendRequestRepository.GetOutgoingRequestsAsync(currentUserId);
                if (requests == null)
                {
                    return Result<IEnumerable<ProfileBasicInfoDto>>.Success(new List<ProfileBasicInfoDto>());
                }
                var requestDtos = requests.Select(r => r.Receiver.ToProfileBasicInfoDto(currentUserId, RelationshipStatus.SentRequest));
                _logger.LogInformation("Fetched Outgoing Friend Requests for user {UserId} successfully", currentUserId);
                return Result<IEnumerable<ProfileBasicInfoDto>>.Success(requestDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while Getting Outgoing Requests");
                return Result<IEnumerable<ProfileBasicInfoDto>>.Failure("Error occured while Getting Outgoing Requests");
            }
        }

        public async Task<Result> RemoveFriendRequestAsync(string senderId, string receiverId)
        {
            try
            {
                var result = await _friendRequestRepository.GetFriendRequestAsync(senderId, receiverId);
                if(result == null)
                {
                    return Result.Failure("Friend Request does not exist");
                }
                _friendRequestRepository.Remove(result);
                await _friendRequestRepository.SaveChangesAsync();

                await _notificationService.DeleteNotificationAsync(senderId, NotificationType.FriendRequest, receiverId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while Removing Friend Request");
                return Result.Failure("Error occured while Removing Friend Request");
            }
        }

        public async Task<Result> SendFriendRequestAsync(string senderId, string receiverId)
        {
            try
            {
                var existingFriendship = await _friendshipRepository.SingleOrDefaultAsync(f => f.FriendId == senderId && f.UserId == receiverId);
                if(existingFriendship != null)
                {
                    return Result.Failure("Friendship is already exist");
                }
                var existingRequest = await _friendRequestRepository.GetFriendRequestAsync(senderId, receiverId);
                if (existingRequest != null)
                {
                    return Result.Failure("Friend Request already sent");
                }
                
                var request = new FriendRequest
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                };  

                await _friendRequestRepository.AddAsync(request);
                await _friendRequestRepository.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(senderId, NotificationType.FriendRequest, receiverId);

                _logger.LogInformation("User {sender} Sent a Friend Request for User {receiver}", senderId, receiverId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while Sending Friend Request");
                return Result.Failure("Error occured while Sending Friend Request");
            }
        }

        public async Task<Result> AcceptFriendRequestAsync(string friendId, string currentUserId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var removingFriendReq = await RemoveFriendRequestAsync(friendId, currentUserId);
                var addingFriendship = await _friendshipService.AddFriendshipAsync(friendId, currentUserId);

                if (!removingFriendReq.IsSuccess || !addingFriendship.IsSuccess) {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure("Error occured while Accepting Friend Request");
                }

                await _unitOfWork.CommitTransactionAsync();

                await _notificationService.SendNotificationAsync(currentUserId, NotificationType.AcceptedFriendRequest, friendId);

                _logger.LogInformation("User {currentUser} has Accepted a Friend Request from User {sender}", currentUserId, friendId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error occured while Accepting Friend Request");
                return Result.Failure("Error occured while Accepting Friend Request");
            }
        }
    }
}

