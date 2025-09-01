using Socialify.Application.DTOs.Account;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<ProfileDto>> LoginAsync(LoginDto loginDto);
        Task<Result<ProfileDto>> RegisterAsync(RegisterDto registerDto, CompleteProfileDto completeProfileDto);
        Task<Result> LogoutAsync();
        Task<Result<ProfileDto>> GetCurrentUserAsync();
        Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
