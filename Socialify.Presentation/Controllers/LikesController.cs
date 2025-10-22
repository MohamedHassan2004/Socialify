using Microsoft.AspNetCore.Mvc;
using Socialify.Application.Services_Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace Socialify.Presentation.Controllers
{
    public class LikesController : BaseController
    {
        private readonly ILikeService _likeService;

        private readonly int PageSize = 5;

        public LikesController(ILogger<LikesController> logger, ILikeService likeService) : base(logger)
        {
            _likeService = likeService;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var result = await _likeService.ToggleLikeAsync(currentUserId, postId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetLikesOnPost(int postId, int pageNumber)
        {
            var result = await _likeService.GetLikesOnPostAsync(postId, currentUserId, pageNumber, PageSize);
            if (!result.IsSuccess)
            {
                HandleServiceError(result, nameof(GetLikesOnPost));
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadLikesOnPost(int postId, int pageNumber)
        {
            var result = await _likeService.GetLikesOnPostAsync(postId, currentUserId, pageNumber, PageSize);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_ProfileList");
            }
            return PartialView("_ProfileList", result.Data!.Data);
        }
    }
}
