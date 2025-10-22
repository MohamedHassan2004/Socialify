using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Home;
using Socialify.Application.Interfaces;
using Socialify.Application.Services;
using Socialify.Application.Services_Interfaces;
using Socialify.Presentation.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Socialify.Presentation.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHomePageService _homePageService;
        private const int pageSize = 5;

        public HomeController(IHomePageService homePageService, ILogger<HomeController> logger) : base(logger)
        {
            _homePageService = homePageService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _homePageService.GetHomePageAsync(currentUserId, pageSize);
            if (!result.IsSuccess)
            {
                return HandleServiceError(result, nameof(Index), "Failed to load home page data. Please try again.");
            }

            ViewData["Title"] = "Home Page";
            return View(result.Data);
        }

        public IActionResult Explore()
        {
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
