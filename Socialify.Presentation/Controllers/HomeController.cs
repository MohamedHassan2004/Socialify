using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Home;
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
        private readonly IHomePageService _homePageService;

        public HomeController(IHomePageService homePageService)
        {
            _homePageService = homePageService;
        }

        public async Task<IActionResult> Index()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction(nameof(AccountController.Login), nameof(AccountController).Replace("Controller", ""));
            }
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["ErrorMessage"] = "Unable to identify user. Please log in again.";
                return RedirectToAction(nameof(AccountController.Login), nameof(AccountController).Replace("Controller", ""));
            }

            var result = await _homePageService.GetHomePageAsync(currentUserId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred while loading the home page. Please try again.";
                return View(new HomePageDto());
            }
            
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
