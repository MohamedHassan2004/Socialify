using Microsoft.EntityFrameworkCore;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Repository
{
    public class SearchRepository : ISearchRepository
    {
        private readonly SocialifyDbContext _context;

        public SearchRepository(SocialifyDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ApplicationUser>> SearchUsersAsync(string keyword)
        {
            return await _context.Users
                .Where(u => u.FirstName.ToLower().Contains(keyword) || u.LastName.ToLower().Contains(keyword))
                .AsNoTracking().ToListAsync();
        }

    }
}
