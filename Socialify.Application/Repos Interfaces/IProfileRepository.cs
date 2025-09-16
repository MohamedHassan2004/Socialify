using Socialify.Domain.Entities;
using System.Threading.Tasks;

namespace Socialify.Application.ReposInterfaces
{
    public interface IProfileRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
    }
}
