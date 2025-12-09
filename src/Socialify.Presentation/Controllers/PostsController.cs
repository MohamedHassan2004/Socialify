using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Domain.Common;
using Socialify.Presentation.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Socialify.Presentation.Controllers
{
    [Authorize]
    public class PostsController : BaseController
    {
        private readonly IPostService _postService;
        public PostsController(ILogger<PostsController> logger, IPostService postService) : base(logger)
        {
            _postService = postService;
        }

        [HttpGet]
        public IActionResult UploadPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPost([FromForm] UploadPostDto uploadPostDto)
        {
            if (!ValidateModelAndLogErrors(uploadPostDto, nameof(UploadPost)))
                return View("UploadPost", uploadPostDto);

            if (uploadPostDto.MediaFile == null && string.IsNullOrWhiteSpace(uploadPostDto.Content))
            {
                TempData["ErrorMessage"] = "Post must have either content or media.";
                return View("UploadPost", new UploadPostDto());
            }

            var result = await _postService.UploadPostAsync(currentUserId, uploadPostDto);
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to upload post {currentUserId}: {ErrorMessage}",currentUserId, result.ErrorMessage);
                TempData["ErrorMessage"] = result.ErrorMessage;
                return View("UploadPost", uploadPostDto);
            }

            TempData["SuccessMessage"] = "Post uploaded successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePost(int postId)
        {
            if (postId == 0)
            {
                return RedirectToAction(nameof(UploadPost));
            }

            var result = await _postService.GetPostByIdAsync(postId, currentUserId);
            if (!result.IsSuccess)
            {
                HandleServiceError(result, nameof(UpdatePost), "Failed to load post for preview.");
            }

            return View("UpdatePost", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePost([FromForm] UpdatePostDto model, bool RemoveMedia)
        {
            var mediaFile = model.MediaFile;

            if (string.IsNullOrWhiteSpace(model.Content) &&
                string.IsNullOrWhiteSpace(model.MediaUrl) &&
                mediaFile == null)
            {
                TempData["ErrorMessage"] = "Post must have either content or media.";
                return View("UpdatePost", model);
            }

            if (RemoveMedia &&
                string.IsNullOrWhiteSpace(model.Content) &&
                mediaFile == null)
            {
                TempData["ErrorMessage"] = "Post must have either content or media.";
                return View("UpdatePost", model);
            }

            if (!ValidateModelAndLogErrors(model, nameof(UpdatePost)))
            {
                return View("UpdatePost", model);
            }

            var result = await _postService.UpdatePostAsync(currentUserId, model, RemoveMedia);

            if (!result.IsSuccess)
            {
                return HandleServiceError(result, nameof(UpdatePost), "Failed to update post.");
            }

            TempData["SuccessMessage"] = "Post updated successfully!";
            return RedirectToAction("GetPostWithComments", "Posts", new { postId = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var result = await _postService.DeletePostAsync(currentUserId, postId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> LoadRelevantPosts([FromQuery] int pageNumber)
        {
            var paramsDto = CreatePaginationParams(pageNumber);

            var result = await _postService.GetRelevantFeedsAsync(paramsDto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_PostList", new List<PostDto>());
            }

            return PartialView("_PostList", result.Data!.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadExplorePosts([FromQuery] int pageNumber)
        {
            var paramsDto = CreatePaginationParams(pageNumber);

            var result = await _postService.GetPagedPostsAsync(paramsDto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_PostList", new List<PostDto>());
            }

            return PartialView("_PostList", result.Data!.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadUserPosts([FromQuery]string userId, [FromQuery] int pageNumber)
        {
            var paramsDto = CreatePaginationParams(pageNumber);

            var result = await _postService.GetPostsByUserIdAsync(userId, paramsDto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_PostList", new List<PostDto>());
            }

            return PartialView("_PostList", result.Data!.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostWithComments(int postId)
        {
            var result = await _postService.GetPostWithCommentsAsync(postId, currentUserId);
            if (!result.IsSuccess)
            {
                HandleServiceError(result, nameof(GetPostWithComments));
            }
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SharePost([FromForm] SharePostDto sharePostDto)
        {
            _logger.LogInformation("SharePost action called. OriginalPostId: {OriginalPostId}, UserId: {UserId}", 
            sharePostDto?.OriginalPostId, currentUserId);

            if (sharePostDto == null)
            {
                _logger.LogError("SharePostDto is null");
                return BadRequest("Share data is null.");
            }

            if (!ValidateModelAndLogErrors(sharePostDto, nameof(SharePost)))
            {
                _logger.LogError("Model validation failed for SharePost");
                return BadRequest("Invalid share data.");
            }

            var result = await _postService.SharePostAsync(currentUserId, sharePostDto);
 
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to share post: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            _logger.LogInformation("Post shared successfully by user {UserId}", currentUserId);
            return Ok(new { message = "Post shared successfully!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsharePost(int sharedPostId)
        {
            var result = await _postService.UnsharePostAsync(currentUserId, sharedPostId);
            
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to unshare post: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            return Ok(new { message = "Shared post removed successfully!" });
        }
    }
}
