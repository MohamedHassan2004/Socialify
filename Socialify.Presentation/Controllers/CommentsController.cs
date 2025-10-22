using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Comment;
using Socialify.Application.Services_Interfaces;

namespace Socialify.Presentation.Controllers
{
    public class CommentsController : BaseController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ILogger<CommentsController> logger, ICommentService commentService) : base(logger)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(AddCommentDto dto)
        {
            if(!ValidateModelAndLogErrors(dto, nameof(AddComment)))
                return BadRequest("Invalid comment data.");

            var result = await _commentService.AddCommentAsync(dto, currentUserId);
            if(!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return PartialView("_Comment", result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> EditComment([FromBody] EditCommentDto dto)
        {
            if (!ValidateModelAndLogErrors(dto, nameof(EditComment)))
                return BadRequest("Invalid comment data.");

            var result = await _commentService.EditCommentAsync(dto, currentUserId);

            if (result.IsSuccess)
                return Ok("Comment edited successfully");

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var result = await _commentService.DeleteCommentAsync(commentId, currentUserId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }
    }
}
