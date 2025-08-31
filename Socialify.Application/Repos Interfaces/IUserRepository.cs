using Socialify.Domain.Entities;
using System.Threading.Tasks;

namespace Socialify.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task UpdateAsync(ApplicationUser user);
    }
}
