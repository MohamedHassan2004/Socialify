using Microsoft.AspNetCore.Mvc;
using Socialify.Application.Services_Interfaces;

namespace Socialify.Presentation.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger) : base(logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var notificationsResult = await _notificationService.GetUserNotificationsAsync(currentUserId);
            if (!notificationsResult.IsSuccess)
            {
                return HandleServiceError(notificationsResult, nameof(Index), "Failed Getting Notifications");
            }
            return View(notificationsResult.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var notificationsResult = await _notificationService.GetUserNotificationsAsync(currentUserId);
            return Json(new { 
                isSuccess = notificationsResult.IsSuccess, 
                data = notificationsResult.Data,
                error = notificationsResult.ErrorMessage 
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var notificationsResult = await _notificationService.GetUserNotificationsAsync(currentUserId);
            if (!notificationsResult.IsSuccess)
            {
                return HandleServiceError(notificationsResult, nameof(GetMyNotifications), "Failed Getting Notifications");
            }
            return View(notificationsResult.Data);
        }
    }
}
