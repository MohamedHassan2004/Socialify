using Humanizer;
using Microsoft.Extensions.Logging;
using Socialify.Application.Automapper;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services
{
    public class SavedPostService : ISavedPostService
    {
        private readonly ISavedPostRepository _savedPostRepository;
        private readonly ILogger<SavedPostService> _logger;

        public SavedPostService(ISavedPostRepository savedPostRepository, ILogger<SavedPostService> logger)
        {
            _savedPostRepository = savedPostRepository;
            _logger = logger;
        }

        public async Task<Result<PagedResult<PostDto>>> GetSavedPostsAsync(string userId, int pageNumber, int pageSize)
        {
            try
            {
                var savedPosts = await _savedPostRepository.GetSavedPostsAsync(userId, pageNumber, pageSize);
                if (savedPosts == null || !savedPosts.Data.Any())
                {
                    return Result<PagedResult<PostDto>>.Success(new PagedResult<PostDto>
                    {
                        Data = new List<PostDto>(),
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = 0
                    });
                }

                var savedPostsDto = savedPosts.Data.Select(sp => new PostDto
                {
                    Id = sp.Post.Id,
                    TimeAgo = sp.Post.CreatedAt.Humanize(false),
                    CreatedAt = sp.Post.CreatedAt,
                    Content = sp.Post.Content,
                    MediaUrl = sp.Post.MediaUrl,
                    MediaType = PostMapper.GetMediaType(sp.Post.MediaUrl),
                    UserId = sp.Post.UserId,
                    UserName = sp.Post.User.FullName,
                    UserProfilePicUrl = sp.Post.User.ProfilePicUrl,
                    LikesCount = sp.Post.LikesCount,
                    CommentsCount = sp.Post.CommentsCount,
                    IsOwnedByCurrentUser = sp.Post.UserId == userId,
                    IsLikedByCurrentUser = sp.Post.Likes.Any(l => l.UserId == userId),
                    IsSavedByCurrentUser = true
                }).ToList();

                var pagedResult = new PagedResult<PostDto>
                {
                    Data = savedPostsDto,
                    PageNumber = savedPosts.PageNumber,
                    PageSize = savedPosts.PageSize,
                    TotalCount = savedPosts.TotalCount
                };

                return Result<PagedResult<PostDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while retrieving saved posts.");
                return Result<PagedResult<PostDto>>.Failure("An error occurred while retrieving saved posts.");
            }
        }

        public async Task<Result> ToggleSavePost(string userId, int postId)
        {
            try
            {
                var existingSavedPost = await _savedPostRepository.SingleOrDefaultAsync(sp => sp.UserId == userId && sp.PostId == postId);
                if (existingSavedPost != null)
                {
                    _savedPostRepository.Remove(existingSavedPost);
                }
                else
                {
                    var newSavedPost = new SavedPost
                    {
                        UserId = userId,
                        PostId = postId,
                    };
                    await _savedPostRepository.AddAsync(newSavedPost);
                }
                await _savedPostRepository.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while toggling the saved post.");
                return Result.Failure("An error occurred while toggling the saved post.");
            }
        }
    }
}
