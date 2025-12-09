using Microsoft.AspNetCore.Mvc;
using Socialify.Application.Services_Interfaces;

namespace Socialify.Presentation.Controllers
{
    public class FriendshipController : BaseController
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IFriendRequestService _friendRequestService;

        public FriendshipController(ILogger<FriendshipController> logger, IFriendshipService friendshipService, IFriendRequestService friendRequestService) : base(logger)
        {
            _friendshipService = friendshipService;
            _friendRequestService = friendRequestService;
        }


        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string friendUserId)
        {
            var result = await _friendRequestService.SendFriendRequestAsync(currentUserId, friendUserId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(string friendUserId)
        {
            var rejectResult = await _friendRequestService.RemoveFriendRequestAsync(friendUserId, currentUserId);
            if (rejectResult.IsSuccess)
            {
                _logger.LogInformation("User {user1} has rejected friend request of User {user2}", currentUserId, friendUserId);
                return Ok();
            }

            return BadRequest("Friend request not found or could not be processed.");
        }

        [HttpPost]
        public async Task<IActionResult> CancelFriendRequest(string friendUserId)
        {
            var rejectResult = await _friendRequestService.RemoveFriendRequestAsync(currentUserId, friendUserId);
            if (rejectResult.IsSuccess)
            {
                _logger.LogInformation("User {user1} has canceled his/her friend request of User {user2}", currentUserId, friendUserId);
                return Ok();
            }
            return BadRequest("Friend request not found or could not be processed.");
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(string friendUserId)
        {
            var AcceptingRequestResult = await _friendRequestService.AcceptFriendRequestAsync(friendUserId, currentUserId);
            if (!AcceptingRequestResult.IsSuccess)
            {
                return BadRequest(AcceptingRequestResult.ErrorMessage);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string friendUserId)
        {
            var result = await _friendshipService.RemoveFriendshipAsync(currentUserId, friendUserId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyFriends()
        {
            var paramsDto = CreatePaginationParams(1);

            var result = await _friendshipService.GetMyFriendshipsAsync(paramsDto);
            if (!result.IsSuccess)
            {
                return HandleServiceError(result, "getting friends");
            }
            ViewBag.UserId = currentUserId;
            return View("GetFriendsByUserId", result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendsByUserId(string userId)
        {
            var paramsDto = CreatePaginationParams(1);

            var result = await _friendshipService.GetFriendshipsByUserIdAsync(userId, paramsDto);
            if (!result.IsSuccess)
            {
                return HandleServiceError(result, "getting friends");
            }
            ViewBag.UserId = userId;
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMyFriends(string userId, int pageNumber)
        {
            var paramsDto = CreatePaginationParams(pageNumber);

            var result = await _friendshipService.GetMyFriendshipsAsync(paramsDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadFriendsByUserId(string userId, int pageNumber)
        {
            var paramsDto = CreatePaginationParams(pageNumber);

            var result = await _friendshipService.GetFriendshipsByUserIdAsync(userId, paramsDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetIncomingFriendRequests()
        {
            var result = await _friendRequestService.GetIncomingRequestsAsync(currentUserId);
            if (!result.IsSuccess)
            {
                return HandleServiceError(result, "getting friend requests");
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetOutgoingFriendRequests()
        {
            var result = await _friendRequestService.GetOutgoingRequestsAsync(currentUserId);
            if (!result.IsSuccess)
            {
                return HandleServiceError(result, "getting friend requests");
            }
            return View(result.Data);
        }
    }
}
