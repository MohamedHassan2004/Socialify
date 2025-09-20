using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Domain.Common;
using System.Security.Claims;

namespace Socialify.Presentation.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected string? GetCurrentUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No user ID found in claims for user {User}", User.Identity?.Name);
            }
            return userId;
        }

        protected IActionResult HandleUnauthorizedAccess(string actionName)
        {
            _logger.LogWarning("Unauthorized access attempt to {ActionName}", actionName);
            return Unauthorized();
        }

        protected IActionResult HandleServiceError<T>(Result<T> result, string actionName, string? fallbackMessage = null)
        {
            _logger.LogError("Service error in {ActionName}: {ErrorMessage}", actionName, result.ErrorMessage);
            TempData["ErrorMessage"] = result.ErrorMessage ?? fallbackMessage ?? "An error occurred. Please try again.";
            return View();
        }

        protected IActionResult HandleServiceError(Result result, string actionName, string? fallbackMessage = null)
        {
            _logger.LogError("Service error in {ActionName}: {ErrorMessage}", actionName, result.ErrorMessage);
            TempData["ErrorMessage"] = result.ErrorMessage ?? fallbackMessage ?? "An error occurred. Please try again.";
            return View();
        }

        protected IActionResult HandleUnexpectedError(Exception ex, string actionName, string? fallbackMessage = null)
        {
            _logger.LogError(ex, "Unexpected error in {ActionName}", actionName);
            TempData["ErrorMessage"] = fallbackMessage ?? "An unexpected error occurred. Please try again.";
            return View();
        }

        protected bool ValidateModelAndLogErrors<T>(T model, string actionName)
        {
            if (model == null)
            {
                _logger.LogWarning("Null model received in {ActionName}", actionName);
                TempData["ErrorMessage"] = "Invalid data provided.";
                return false;
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed in {ActionName}. Errors: {Errors}", 
                    actionName, string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return false;
            }

            return true;
        }
    }
}
