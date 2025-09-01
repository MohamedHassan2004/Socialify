using Socialify.Application.DTOs.Account;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System.Threading.Tasks;

namespace Socialify.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<ApplicationUser>> GetCurrentUserAsync();
        Task<Result> UpdateUserAsync(ApplicationUser user, CompleteProfileDto model);
    }
}
