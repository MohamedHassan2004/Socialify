using Humanizer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Comment;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.Interfaces;
using Socialify.Application.Mappers;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Domain.Events;

namespace Socialify.Application.Services;
public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostService> _logger;
    private readonly IFileManager _fileManager;
    private readonly IConfiguration _config;
    private readonly ISharedPostRepository _sharedPostRepository;
    private readonly IMediator _mediator;
    private readonly string _postMediaPath;

    public PostService(
        IPostRepository postRepository, 
        ILogger<PostService> logger, 
        IFileManager fileManager, 
        IConfiguration config,
        ISharedPostRepository sharedPostRepository,
        IMediator mediator)
    {
        _postRepository = postRepository;
        _logger = logger;
        _fileManager = fileManager;
        _config = config;
        _sharedPostRepository = sharedPostRepository;
        _mediator = mediator;
        _postMediaPath = config["FileSettings:PostMediaPath"] ?? "posts";
    }

    public async Task<Result> UploadPostAsync(string userId, UploadPostDto uploadPostDto)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(uploadPostDto.Content) && uploadPostDto.MediaFile == null)
            {
                return Result.Failure("Post must have either content or media.");
            }

            var post = new Post
            {
                Content = uploadPostDto.Content,
                UserId = userId,
            };

            if (uploadPostDto.MediaFile != null)
            {
                var savingMediaResult = await _fileManager.SaveFileAsync(uploadPostDto.MediaFile, _postMediaPath);
                if (!savingMediaResult.IsSuccess)
                {
                    return Result.Failure(savingMediaResult.ErrorMessage);
                }
                post.MediaUrl = savingMediaResult.Data;
            }

            await _postRepository.AddAsync(post);
            var isAdded = await _postRepository.SaveChangesAsync() > 0;
            if (!isAdded)
            {
                _logger.LogError("Failed to save post to database for user {UserId}", userId);
                return Result.Failure("Failed to save post. Please try again.");
            }

            _logger.LogInformation("User {UserId} uploaded a new post {PostId}.", userId, post.Id);
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while uploading post.");
            return Result.Failure("Error occurred while uploading post.");
        }
    }

    public async Task<Result> UpdatePostAsync(string userId, UpdatePostDto updatePostDto, bool removeMedia)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(updatePostDto.Id);
            if (post == null || post.UserId != userId)
                return Result.Failure("Post not found.");

            post.Content = updatePostDto.Content;

            if (removeMedia)
            {
                if (!string.IsNullOrEmpty(post.MediaUrl))
                {
                    _fileManager.DeleteFile(post.MediaUrl);
                    post.MediaUrl = null;
                }
            }
            else if (updatePostDto.MediaFile != null)
            {
                if (!string.IsNullOrEmpty(post.MediaUrl))
                    _fileManager.DeleteFile(post.MediaUrl);

                var fileName = await _fileManager.SaveFileAsync(updatePostDto.MediaFile, _postMediaPath);
                post.MediaUrl = fileName.Data;
            }

            post.IsEdited = true;

            _postRepository.Update(post);
            var success = await _postRepository.SaveChangesAsync() > 0;
            if (!success)
                return Result.Failure("Failed to update post in database.");

            _logger.LogInformation("Post {PostId} updated successfully.", post.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error updating post: {ex.Message}");
        }
    }

    public async Task<Result> DeletePostAsync(string currentUserId, int postId)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return Result.Failure("Post not found.");
            }
            if (post.UserId != currentUserId)
            {
                return Result.Failure("You are not authorized to delete this post.");
            }
            if (!string.IsNullOrEmpty(post.MediaUrl))
            {
                var deleteFileResult = _fileManager.DeleteFile(post.MediaUrl);
                if (!deleteFileResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete media file at {MediaUrl}: {ErrorMessage}", post.MediaUrl, deleteFileResult.ErrorMessage);
                }
            }

            await _mediator.Publish(new OriginalPostDeletingEvent(post.Id));

            _postRepository.Remove(post);
            await _postRepository.SaveChangesAsync();
            _logger.LogInformation("Post {PostId} deleted successfully by user {UserId}.", post.Id, currentUserId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting post.");
            return Result.Failure("Error occurred while deleting post.");
        }
    }

    public Task<Result<PagedResult<PostDto>>> GetPagedPostsAsync(PaginationParamsDto paramsDto) =>
        HandlePagedOperation(() => _postRepository.GetPagedPostsAsync(paramsDto.PageNumber, paramsDto.PageSize),"fetching paged posts", paramsDto.CurrentUserId);

    public Task<Result<PagedResult<PostDto>>> SearchPostsAsync(string query, PaginationParamsDto paramsDto) =>
        HandlePagedOperation(() => _postRepository.SearchPostsAsync(query, paramsDto.PageNumber, paramsDto.PageSize),"searching posts", paramsDto.CurrentUserId);

    public Task<Result<PagedResult<PostDto>>> GetPostsByUserIdAsync(string userId, PaginationParamsDto paramsDto) =>
        HandlePagedOperation(() => _postRepository.GetPostsByUserId(userId, paramsDto.PageNumber, paramsDto.PageSize), "fetching posts by user", paramsDto.CurrentUserId);

    public async Task<Result<UpdatePostDto>> GetPostByIdAsync(int postId, string currentUserId)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return Result<UpdatePostDto>.Failure("Post not found.");
            }

            var updatePostDto = post.ToUpdatePostDto();

            return Result<UpdatePostDto>.Success(updatePostDto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching post by ID.");
            return Result<UpdatePostDto>.Failure("Error occurred while fetching post by ID.");
        }   
    }


    public async Task<Result<PostWithDetailsDto>> GetPostWithCommentsAsync(int postId, string currentUserId)
    {
        try
        {
            var post = await _postRepository.GetPostWithCommentsAsync(postId);
            if (post == null)
            {
                return Result<PostWithDetailsDto>.Failure("Post not found.");
            }
            var postDto = post.ToPostDto(currentUserId);
            var commentsDto = post.Comments.Select(c => c.ToCommentDtoWithCurrentUser(currentUserId)).ToList();

            return Result<PostWithDetailsDto>.Success(new PostWithDetailsDto()
            {
                Post = postDto,
                Comments = commentsDto
            }); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching post with comments.");
            return Result<PostWithDetailsDto>.Failure("Error occurred while fetching post with comments.");
        }
    }

    // ---------- Private Helpers ----------
    private async Task<Result<PagedResult<PostDto>>> HandlePagedOperation(
        Func<Task<PagedResult<Post>>> repositoryCall,
        string operationDescription,
        string currentUserId)
    {
        try
        {
            var postsPaged = await repositoryCall();
            var postDtos = postsPaged.Data.Select(p => p.ToPostDto(currentUserId)).ToList();

            var dtoPaged = new PagedResult<PostDto>
            {
                Data = postDtos,
                PageNumber = postsPaged.PageNumber,
                PageSize = postsPaged.PageSize,
                TotalCount = postsPaged.TotalCount
            };

            return Result<PagedResult<PostDto>>.Success(dtoPaged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while {operationDescription}.");
            return Result<PagedResult<PostDto>>.Failure($"Error occurred while {operationDescription}.");
        }
    }


    public async Task<Result> SharePostAsync(string userId, SharePostDto sharePostDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure("User ID is required.");

            // Get the original post
            var originalPost = await _postRepository.GetByIdAsync(sharePostDto.OriginalPostId);
            if (originalPost == null)
                return Result.Failure("Original post not found.");

            // Prevent sharing own post
            if (originalPost.UserId == userId)
                return Result.Failure("You cannot share your own post.");

            // Check if already shared - FIXED: Check against the actual original post ID
            var actualOriginalPostId = originalPost.IsShared && originalPost.OriginalPostId.HasValue
                ? originalPost.OriginalPostId.Value
                : originalPost.Id;

            var alreadyShared = await _sharedPostRepository.UserHasSharedPostAsync(actualOriginalPostId, userId);
            if (alreadyShared)
                return Result.Failure("You have already shared this post.");

            // Create the shared post
            var sharedPost = new Post
            {
                Content = sharePostDto.AdditionalComment,
                UserId = userId,
                IsShared = true,
                OriginalPostId = actualOriginalPostId, 
                CreatedAt = DateTime.Now
            };

            await _postRepository.AddAsync(sharedPost);
            var saved = await _postRepository.SaveChangesAsync() > 0;

            if (!saved)
            {
                _logger.LogError("Failed to save shared post to database for user {UserId}", userId);
                return Result.Failure("Failed to share post. Please try again.");
            }

            // Create SharedPost tracking entry
            var sharedPostTracking = new SharedPost
            {
                OriginalPostId = actualOriginalPostId,
                SharedPostId = sharedPost.Id,
                SharedByUserId = userId,
                SharedAt = DateTime.Now
            };

            await _sharedPostRepository.AddAsync(sharedPostTracking);

            // Increment share count on the actual original post
            var postToUpdate = await _postRepository.GetByIdAsync(actualOriginalPostId);
            if (postToUpdate != null)
            {
                postToUpdate.IncrementSharesCount();
                _postRepository.Update(postToUpdate);
            }

            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("User {UserId} shared post {PostId}.", userId, sharePostDto.OriginalPostId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sharing post.");
            return Result.Failure("Error occurred while sharing post.");
        }
    }

    public async Task<Result> UnsharePostAsync(string userId, int sharedPostId)
    {
        try
        {
            var sharedPost = await _postRepository.GetByIdAsync(sharedPostId);
            if (sharedPost == null || !sharedPost.IsShared)
                return Result.Failure("Shared post not found.");

            if (sharedPost.UserId != userId)
                return Result.Failure("You are not authorized to unshare this post.");

            // Get the tracking entry
            var sharedPostTracking = await _sharedPostRepository.GetByOriginalAndSharedPostIdsAsync(
            sharedPost.OriginalPostId!.Value, sharedPostId);

            if (sharedPostTracking != null)
            {
                _sharedPostRepository.Remove(sharedPostTracking);
            }

            // Decrement share count
            var originalPost = await _postRepository.GetByIdAsync(sharedPost.OriginalPostId!.Value);
            if (originalPost != null)
            {
                originalPost.DecrementSharesCount();
                _postRepository.Update(originalPost);
            }

            // Delete the shared post
            _postRepository.Remove(sharedPost);
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("User {UserId} unshared post {PostId}.", userId, sharedPostId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while unsharing post.");
            return Result.Failure("Error occurred while unsharing post.");
        }
    }
}