using Socialify.Application.DTOs.Comment;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Domain.Common;
using System.Threading.Tasks;

namespace Socialify.Application.Interfaces;

public interface IPostService
{
    Task<Result> UploadPostAsync(string userId, UploadPostDto uploadPostDto);
    Task<Result> UpdatePostAsync(string userId, UpdatePostDto updatePostDto, bool removeMedia);
    Task<Result> DeletePostAsync(string userId, int postId);
    Task<Result> SharePostAsync(string userId, SharePostDto sharePostDto);
    Task<Result> UnsharePostAsync(string userId, int sharedPostId);
    Task<Result<UpdatePostDto>> GetPostByIdAsync(int postId, string currentUserId);
    Task<Result<PagedResult<PostDto>>> GetPagedPostsAsync(PaginationParamsDto paramsDto);
    Task<Result<PagedResult<PostDto>>> GetPostsByUserIdAsync(string userId, PaginationParamsDto paramsDto);
    Task<Result<PagedResult<PostDto>>> SearchPostsAsync(string query, PaginationParamsDto paramsDto);
    Task<Result<PostWithDetailsDto>> GetPostWithCommentsAsync(int postId, string currentUserId);
}
