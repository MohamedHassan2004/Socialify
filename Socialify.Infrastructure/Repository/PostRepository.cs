using Microsoft.EntityFrameworkCore;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.Repos_Interfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private IQueryable<Post> BaseQuery()
        {
            return _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.SavedPosts)
                .AsSplitQuery()
                .AsNoTracking();
        }

        public async Task<PagedResult<Post>> GetPagedPostsAsync(int pageNumber, int pageSize)
        {
            return await BaseQuery()
                .OrderByDescending(p => p.CreatedAt)
                .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<Post>> SearchPostsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            return await BaseQuery()
                .Where(p => EF.Functions.Like(p.Content!, $"%{searchTerm}%"))
                .OrderByDescending(p => p.CreatedAt)
                .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<Post>> GetPostsByUserId(string userId, int pageNumber, int pageSize)
        {
            return await BaseQuery()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<Post?> GetPostWithCommentsAsync(int postId)
        {
            return await BaseQuery()
                .Where(p => p.Id == postId)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();  
        }
    }
}
