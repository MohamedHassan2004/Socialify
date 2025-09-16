using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Presentation.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Socialify.Presentation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomePageService _homePageService;

        public HomeController(ILogger<HomeController> logger, IHomePageService homePageService)
        {
            _logger = logger;
            _homePageService = homePageService;
        }

        public async Task<IActionResult> Index()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction(nameof(AccountController.Login), nameof(AccountController).Replace("Controller", ""));
            }
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _homePageService.GetHomePageAsync(currentUserId);

            if (!result.IsSuccess)
                return View("Error", result.ErrorMessage);

            ViewData["Title"] = "Home Page";
            return View(result.Data);
        }

        public IActionResult Explore()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction(nameof(AccountController.Login), nameof(AccountController).Replace("Controller", ""));
            }
            ViewData["Title"] = "Explore";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
