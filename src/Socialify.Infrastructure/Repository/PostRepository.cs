using Microsoft.EntityFrameworkCore;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly SocialifyDbContext _context;

        public PostRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<TResult>> GetFeedsAsync<TResult>(
            Expression<Func<Post, bool>> filter,
            Expression<Func<Post, TResult>> projection,
            int pageNumber,
            int pageSize,
            Expression<Func<Post, object>>? orderByDescending = null)
        {
            var query = _context.Posts
                .AsNoTracking()
                .Where(filter);

            if (orderByDescending != null)
            {
                query = query.OrderByDescending(orderByDescending);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(projection)
                .ToListAsync();

            return new PagedResult<TResult>(items, totalCount, pageNumber, pageSize);
        }


        public async Task<PagedResult<TResult>> SearchPostsAsync<TResult>(Expression<Func<Post,TResult>> projection,string searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Posts
                .Where(p =>
                    EF.Functions.Like(p.Content ?? "", $"%{searchTerm}%") ||
                    (p.IsShared && p.OriginalPost != null && EF.Functions.Like(p.OriginalPost.Content ?? "", $"%{searchTerm}%"))
                )
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(projection)
                .ToListAsync();

            return new PagedResult<TResult>(items, totalCount, pageNumber, pageSize);
        }

        public Task<TResult?> GetPostWithCommentsAsync<TResult>(Expression<Func<Post, TResult>> projection, int postId)
        {
            return _context.Posts
                .AsNoTracking()
                .Where(p => p.Id == postId)
                .Select(projection)
                .FirstOrDefaultAsync();
        }
    }
}
