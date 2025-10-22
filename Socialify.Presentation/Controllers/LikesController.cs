using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Common;
using Socialify.Application.Services_Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace Socialify.Presentation.Controllers
{
    public class LikesController : BaseController
    {
        private readonly ILikeService _likeService;

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
        public async Task<IActionResult> GetLikesOnPost(int postId)
        {
            var paramsDto = CreatePaginationParams(1);

            var result = await _likeService.GetLikesOnPostAsync(postId, paramsDto);
            if (!result.IsSuccess)
            {
                HandleServiceError(result, nameof(GetLikesOnPost));
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadLikesOnPost([FromQuery] int postId,[FromQuery] int pageNumber)
        {
            var paramsDto = CreatePaginationParams(pageNumber);

            var result = await _likeService.GetLikesOnPostAsync(postId, paramsDto);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_ProfileList");
            }
            return PartialView("_ProfileList", result.Data!.Data);
        }
    }
}
