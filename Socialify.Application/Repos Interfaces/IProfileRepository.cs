using Socialify.Domain.Entities;
using System.Threading.Tasks;

namespace Socialify.Application.RepoInterfaces
{
    public interface IProfileRepository
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task UpdateAsync(ApplicationUser user);
    }
}
