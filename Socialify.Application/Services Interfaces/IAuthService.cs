using Socialify.Application.DTOs.Account;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result> LoginAsync(LoginDto loginDto);
        Task<Result> RegisterAsync(RegisterDto registerDto, CompleteProfileDto completeProfileDto);
        Task<Result> LogoutAsync();
        Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<Result> DeleteAccountAsync(string userId);
        Task<Result<ProfileDto>> GetCurrentUserAsync();
        Task<bool> IsEmailExistsAsync(string email);
    }
}
