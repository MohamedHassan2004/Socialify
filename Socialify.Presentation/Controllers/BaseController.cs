using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Domain.Common;
using Socialify.Presentation.Filters;
using System.Security.Claims;

namespace Socialify.Presentation.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(RequireUserIdFilter))]
    public abstract class BaseController : Controller
    {
        protected readonly ILogger _logger;
        protected string currentUserId => HttpContext.Items["CurrentUserId"]?.ToString()!;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
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
            return View("Error");
        }

        protected IActionResult HandleServiceError(Result result, string actionName, string? fallbackMessage = null)
        {
            _logger.LogError("Service error in {ActionName}: {ErrorMessage}", actionName, result.ErrorMessage);
            TempData["ErrorMessage"] = result.ErrorMessage ?? fallbackMessage ?? "An error occurred. Please try again.";
            return View("Error");
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
                var errors = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("Model validation failed in {ActionName}. Errors: {Errors}", 
                    actionName, errors);
                TempData["ErrorMessage"] = errors;
                return false;
            }

            return true;
        }
    }
}
