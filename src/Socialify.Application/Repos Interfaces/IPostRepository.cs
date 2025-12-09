using Socialify.Application.DTOs.Common;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using System.Linq;

namespace Socialify.Application.Repos_Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<PagedResult<Post>> GetPagedPostsAsync(int pageNumber, int pageSize);
        Task<PagedResult<Post>> SearchPostsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<PagedResult<Post>> GetPostsByUserId(string userId, int pageNumber, int pageSize);
        Task<Post?> GetPostWithCommentsAsync(int postId);
        Task<Post?> GetPostWithOriginalPostAsync(int postId);
    }
}

