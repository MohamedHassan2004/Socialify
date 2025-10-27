using Microsoft.EntityFrameworkCore;
using Socialify.Application.DTOs.Common;
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
    public class SavedPostRepository : Repository<SavedPost>, ISavedPostRepository
    {
        private readonly SocialifyDbContext _context;

        public SavedPostRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<PagedResult<SavedPost>> GetSavedPostsAsync(string userId, int pageNumber, int pageSize)
        {
            return await _context.SavedPosts
                .Where(sp => sp.UserId == userId)
                .OrderByDescending(sp => sp.SavedAt)
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.User)
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.Likes.Where(l => l.UserId == userId))
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.OriginalPost)
                        .ThenInclude(op => op!.User)
                .AsNoTracking()
                .AsSplitQuery()
                .ToPagedResultAsync(pageNumber, pageSize);
        }
    }
}
