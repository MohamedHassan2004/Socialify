using Microsoft.EntityFrameworkCore;
using Socialify.Application.Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System.Linq.Expressions;

namespace Socialify.Infrastructure.Repository
{
    public class ProfileRepository : Repository<ApplicationUser>, IProfileRepository
    {
        private readonly SocialifyDbContext _context;

        public ProfileRepository(SocialifyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _context.FindAsync<ApplicationUser>(userId);
        }

        // New method with eager loading to prevent N+1 queries
        public async Task<ApplicationUser?> GetByIdWithPostsAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Posts)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        // Method to get user with specific related data
        public async Task<ApplicationUser?> GetByIdWithIncludesAsync(string userId, params Expression<Func<ApplicationUser, object>>[] includes)
        {
            var query = _context.Users.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        // Method to get multiple users with related data (for friend lists, etc.)
        public async Task<IEnumerable<ApplicationUser>> GetUsersWithPostsAsync(IEnumerable<string> userIds)
        {
            return await _context.Users
                .Include(u => u.Posts)
                .Where(u => userIds.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        // Method to get user profile data with pagination
        public async Task<(IEnumerable<ApplicationUser> Users, int TotalCount)> GetUsersWithPaginationAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<ApplicationUser, bool>>? filter = null)
        {
            var query = _context.Users
                .Include(u => u.Posts)
                .AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync();
            
            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        // Method to search users with related data
        public async Task<IEnumerable<ApplicationUser>> SearchUsersWithPostsAsync(
            string searchTerm, 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            return await _context.Users
                .Include(u => u.Posts)
                .Where(u => u.FirstName.Contains(searchTerm) || 
                           u.LastName.Contains(searchTerm) || 
                           u.Email.Contains(searchTerm))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
