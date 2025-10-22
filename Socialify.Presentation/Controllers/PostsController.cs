using AutoMapper;
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
        private readonly int PageSize = 5;

        public PostsController(ILogger<PostsController> logger, IPostService postService) : base(logger)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> LoadPosts(int pageNumber)
        {
            var result = await _postService.GetPagedPostsAsync(pageNumber, PageSize, currentUserId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return PartialView("_PostList", new List<PostDto>());
            }

            return PartialView("_PostList", result.Data!.Data);
        }

        [HttpGet]
        public async Task<IActionResult> LoadUserPosts(string userId, int pageNumber)
        {
            var result = await _postService.GetPostsByUserIdAsync(userId, currentUserId, pageNumber, PageSize);

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

            // Manual validation: check if post has content or media
            if (string.IsNullOrWhiteSpace(model.Content) &&
                string.IsNullOrWhiteSpace(model.MediaUrl) &&
                mediaFile == null)
            {
                TempData["ErrorMessage"] = "Post must have either content or media.";
                return View("UpdatePost", model);
            }

            // Check if removing media without adding new content/media
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

            var updatePostDto = new UpdatePostDto
            {
                Id = model.Id,
                Content = model.Content,
                MediaFile = mediaFile,
                RemoveMedia = RemoveMedia,
                UserId = currentUserId
            };

            var result = await _postService.UpdatePostAsync(updatePostDto);

            if (!result.IsSuccess)
            {
                return HandleServiceError(result, nameof(UpdatePost), "Failed to update post.");
            }

            TempData["SuccessMessage"] = "Post updated successfully!";
            return RedirectToAction("GetPostWithComments", "Posts", new { postId = model.Id });
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
    }
}
