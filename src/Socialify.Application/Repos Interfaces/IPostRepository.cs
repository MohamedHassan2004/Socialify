using Socialify.Application.DTOs.Common;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Socialify.Application.Repos_Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<PagedResult<TResult>> GetFeedsAsync<TResult>(
                    Expression<Func<Post, bool>> filter,
                    Expression<Func<Post, TResult>> projection,
                    int pageNumber,
                    int pageSize,
                    Expression<Func<Post, object>>? orderByDescending = null);
        Task<PagedResult<TResult>> SearchPostsAsync<TResult>(Expression<Func<Post, TResult>> projection, string searchTerm, int pageNumber, int pageSize);
        Task<TResult?> GetPostWithCommentsAsync<TResult>(Expression<Func<Post, TResult>> projection, int postId);
        //Task<Post?> GetPostWithOriginalPostAsync(int postId);
    }
}

