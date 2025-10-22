using Socialify.Application.DTOs.Common;
using Socialify.Application.Interfaces;
using Socialify.Domain.Entities;
using System.Linq.Expressions;

namespace Socialify.Application.ReposInterfaces
{
    public interface IProfileRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetUserProfileByIdAsync(string userId);
        Task<PagedResult<ApplicationUser>> SearchUsersAsync(string searchTerm,int pageNumber,int pageSize);
    }
}
