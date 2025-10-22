using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.DTOs.Search;
using Socialify.Application.Interfaces;
using Socialify.Application.Services_Interfaces;

namespace Socialify.Presentation.Controllers
{
    public class SearchController : BaseController
    {
        private readonly ISearchService _searchService;
        private readonly IPostService _postService;
        private readonly IProfileService _profileService;
        private const int PageSize = 3;

        public SearchController(ISearchService searchService,
            ILogger<SearchController> logger,
            IPostService postService,
            IProfileService profileService
            ) : base(logger)
        {
            _searchService = searchService;
            _postService = postService;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string query)
        {
            var result = await _searchService.SearchAsync(query, 1, PageSize, currentUserId);

            if (!result.IsSuccess)
                HandleServiceError(result, nameof(Index), "Failed to load search results. Please try again.");

            ViewBag.Query = query;
            ViewBag.PageSize = PageSize;
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> SearchMorePosts(int pageNumber, string? query = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("This endpoint does not support search queries.");

            var result = await _postService.SearchPostsAsync(query, pageNumber, PageSize, currentUserId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_PostList", new List<PostDto>());
            }

            return PartialView("_PostList", result.Data!.Data);
        }

        [HttpGet]
        public async Task<IActionResult> SearchMoreProfiles(int pageNumber, string? query = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("This endpoint does not support search queries.");

            var result = await _profileService.SearchProfilesAsync(query, pageNumber, PageSize, currentUserId);

            if (!result.IsSuccess)
                return PartialView("_ProfileList", new List<ProfileBasicInfoDto>());

            return PartialView("_ProfileList", result.Data!.Data);
        }
    }
}
