using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Common;
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
        private readonly IPostService _postService;

        public HomeController(IHomePageService homePageService, IPostService postService, ILogger<HomeController> logger) : base(logger)
        {
            _homePageService = homePageService;
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _homePageService.GetHomePageAsync(currentUserId, PageSize);
            if (!result.IsSuccess)
            {
                return HandleServiceError(result, nameof(Index), "Failed to load home page data. Please try again.");
            }

            ViewBag.FriendRequestsCount = result.Data.FriendRequestsCount;
            ViewBag.NotificationsCount = result.Data.NotificationsCount;
            ViewData["Title"] = "Home Page";
            return View(result.Data);
        }

        public async Task<IActionResult> Explore()
        {
            var paramsDto = new PaginationParamsDto
            {
                PageNumber = 1,
                PageSize = PageSize,
                CurrentUserId = currentUserId
            };

            var result = await _postService.GetPagedPostsAsync(paramsDto);
            if (!result.IsSuccess)
            {
                return HandleServiceError(result, nameof(Index), "Failed to load explore page posts. Please try again.");
            }
            ViewData["Title"] = "Explore";
            return View(result.Data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
