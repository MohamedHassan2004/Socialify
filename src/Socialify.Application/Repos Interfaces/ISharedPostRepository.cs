using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;

namespace Socialify.Application.Repos_Interfaces
{
    public interface ISharedPostRepository : IRepository<SharedPost>
    {
        Task<SharedPost?> GetByOriginalAndSharedPostIdsAsync(int originalPostId, int sharedPostId);
        Task<bool> UserHasSharedPostAsync(int originalPostId, string userId);
    }
}
