using Socialify.Application.DTOs.Comment;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Domain.Common;
using System.Threading.Tasks;

namespace Socialify.Application.Interfaces;

public interface IPostService
{
    Task<Result<PagedResult<PostDto>>> GetPagedPostsAsync(int pageNumber, int pageSize, string currentUserId);
    Task<Result<PagedResult<PostDto>>> GetPostsByUserIdAsync(string userId, int pageNumber, int pageSize, string currentUserId);
    Task<Result<PagedResult<PostDto>>> SearchPostsAsync(string query, int pageNumber, int pageSize, string currentUserId);
    Task<Result> UploadPostAsync(string userId, UploadPostDto uploadPostDto);
    Task<Result> DeletePostAsync(string userId, int postId);
    Task<Result<UpdatePostDto>> GetPostByIdAsync(int postId, string currentUserId);
    Task<Result> UpdatePostAsync(UpdatePostDto updatePostDto);
    Task<Result<PostWithDetailsDto>> GetPostWithCommentsAsync(int postId, string currentUserId);
}
