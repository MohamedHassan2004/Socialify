using Socialify.Application.Interfaces;
using Socialify.Domain.Entities;
using System.Linq.Expressions;

namespace Socialify.Application.ReposInterfaces
{
    public interface IProfileRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByIdWithPostsAsync(string userId);
        Task<ApplicationUser?> GetByIdWithIncludesAsync(string userId, params Expression<Func<ApplicationUser, object>>[] includes);
        Task<IEnumerable<ApplicationUser>> GetUsersWithPostsAsync(IEnumerable<string> userIds);
        Task<(IEnumerable<ApplicationUser> Users, int TotalCount)> GetUsersWithPaginationAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<ApplicationUser, bool>>? filter = null);
        Task<IEnumerable<ApplicationUser>> SearchUsersWithPostsAsync(
            string searchTerm, 
            int pageNumber = 1, 
            int pageSize = 10);
    }
}
