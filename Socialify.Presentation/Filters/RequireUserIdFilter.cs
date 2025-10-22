using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Socialify.Presentation.Filters
{
    public class RequireUserIdFilter : IAsyncActionFilter
    {
        private readonly ILogger<RequireUserIdFilter> _logger;

        public RequireUserIdFilter(ILogger<RequireUserIdFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unauthorized access attempt to {ActionName}",
                    context.ActionDescriptor.DisplayName);

                context.Result = new UnauthorizedResult();
                return; // stop pipeline
            }

            // inject userId in HttpContext.Items so controller can use it
            context.HttpContext.Items["CurrentUserId"] = userId;

            await next(); // continue pipeline
        }
    }

}
