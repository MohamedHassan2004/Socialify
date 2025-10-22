using AutoMapper;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Socialify.Application.Automapper;
using Socialify.Application.DTOs.Comment;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.Interfaces;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;

namespace Socialify.Application.Services;
public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PostService> _logger;
    private readonly IFileManager _fileManager;
    private readonly IConfiguration _config;
    private readonly string _postMediaPath;

    public PostService(IPostRepository postRepository, IMapper mapper, ILogger<PostService> logger, IFileManager fileManager, IConfiguration config)
    {
        _postRepository = postRepository;
        _mapper = mapper;
        _logger = logger;
        _fileManager = fileManager;
        _config = config;
        
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
            var IsAdded = await _postRepository.SaveChangesAsync() > 0;
            if (IsAdded)
            {
                _logger.LogInformation("User {UserId} uploaded a new post {PostId}.", userId, post.Id);
                return Result.Success();
            }
            else
            {
                return Result.Failure("Error occurred while uploading post");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while uploading post.");
            return Result.Failure("Error occurred while uploading post.");
        }
    }

    public async Task<Result> UpdatePostAsync(UpdatePostDto updatePostDto)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(updatePostDto.Id);
            if (post == null || post.UserId != updatePostDto.UserId)
                return Result.Failure("Post not found.");

            post.Content = updatePostDto.Content;

            if (updatePostDto.RemoveMedia)
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

    public Task<Result<PagedResult<PostDto>>> GetPagedPostsAsync(int pageNumber, int pageSize, string currentUserId) =>
        HandlePagedOperation(() => _postRepository.GetPagedPostsAsync(pageNumber, pageSize),"fetching paged posts", currentUserId);

    public Task<Result<PagedResult<PostDto>>> SearchPostsAsync(string query, int pageNumber, int pageSize,string currentUserId) =>
        HandlePagedOperation(() => _postRepository.SearchPostsAsync(query, pageNumber, pageSize),"searching posts", currentUserId);

    public Task<Result<PagedResult<PostDto>>> GetPostsByUserIdAsync(string userId, string currentUserId, int pageNumber, int pageSize) =>
        HandlePagedOperation(() => _postRepository.GetPostsByUserId(userId, pageNumber, pageSize),"fetching posts by user", currentUserId);

    public async Task<Result<UpdatePostDto>> GetPostByIdAsync(int postId, string currentUserId)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return Result<UpdatePostDto>.Failure("Post not found.");
            }

            var updatePostDto = new UpdatePostDto
            {
                Id = post.Id,
                Content = post.Content,
                MediaUrl = post.MediaUrl,
                MediaType = PostMapper.GetMediaType(post.MediaUrl),
            };

            return Result<UpdatePostDto>.Success(updatePostDto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching post by ID.");
            return Result<UpdatePostDto>.Failure("Error occurred while fetching post by ID.");
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
            var postDtos = postsPaged.Data.Select(p=> PostMapper.MapPost(p,currentUserId)).ToList();

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

    public async Task<Result<PostWithDetailsDto>> GetPostWithCommentsAsync(int postId, string currentUserId)
    {
        try
        {
            var post = await _postRepository.GetPostWithCommentsAsync(postId);
            if (post == null)
            {
                return Result<PostWithDetailsDto>.Failure("Post not found.");
            }
            var postDto = PostMapper.MapPost(post, currentUserId);
            var commentsDto = post.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                UserId = c.UserId,
                UserName = c.User.FullName,
                UserProfilePictureUrl = c.User.ProfilePicUrl,
                TimeAgo = c.CreatedAt.Humanize(false),
                IsEdited = c.IsEdited,
                CanEditOrDelete = c.UserId == currentUserId
            }).ToList();

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
}