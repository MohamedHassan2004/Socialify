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
                .Include(p => p.SharedPosts)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
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
                .Where(p => 
                    // Search in the post's own content
                    EF.Functions.Like(p.Content ?? "", $"%{searchTerm}%") ||
                    // Search in original post's content if it's a shared post
                    (p.IsShared && p.OriginalPost != null && EF.Functions.Like(p.OriginalPost.Content ?? "", $"%{searchTerm}%"))
                )
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

        public async Task<Post?> GetPostWithOriginalPostAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.SavedPosts)
                .Include(p => p.SharedPosts)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId);  
        }
    }
}
