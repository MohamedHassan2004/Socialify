using Microsoft.EntityFrameworkCore;
using Socialify.Application.DTOs.Comment;
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
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly SocialifyDbContext _context;

        public CommentRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Comment?> GetCommentWithUserAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }
    }
}
