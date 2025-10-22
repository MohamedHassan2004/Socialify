using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.Services_Interfaces;

namespace Socialify.Presentation.Controllers
{
    public class SavedPostsController : BaseController
    {
        private readonly ISavedPostService _savedPostService;
        public SavedPostsController(ILogger<SavedPostsController> logger, ISavedPostService savedPostService) : base(logger)
        {
            _savedPostService = savedPostService;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSavePost(int postId)
        {
            var result = await _savedPostService.ToggleSavePost(currentUserId, postId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSavedPosts()
        {
            var paramsDto = CreatePaginationParams(1);

            var result = await _savedPostService.GetSavedPostsAsync(paramsDto);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return HandleServiceError(result, nameof(GetSavedPosts), "Could not retrieve saved posts.");
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadSavedPosts([FromQuery]int pageNumber)
        {
            var paramsDto = CreatePaginationParams(pageNumber);

            var result = await _savedPostService.GetSavedPostsAsync(paramsDto);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_PostList", new List<PostDto>());
            }
            return PartialView("_PostList", result.Data!.Data);
        }

    }
}
