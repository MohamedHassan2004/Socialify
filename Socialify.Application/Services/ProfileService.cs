using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Account;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.RepoInterfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Socialify.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProfileDto> _logger;

        public ProfileService(IProfileRepository userRepository, IHttpContextAccessor httpContextAccessor, ILogger<ProfileDto> logger)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Result<ApplicationUser>> GetCurrentUserAsync()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Result<ApplicationUser>.Failure("Can't Access HttpContext");
            }

            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Result<ApplicationUser>.Failure("User ID claim not found.");
            }

            var userId = userIdClaim.Value;
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result<ApplicationUser>.Failure("User not found.");
            }
            return Result<ApplicationUser>.Success(user);
        }

        public async Task<Result> UpdateUserAsync(ApplicationUser user, CompleteProfileDto model)
        {
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;
            user.Bio = model.Bio;
            user.BirthDate = model.BirthDate;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("User {UserId} Updated their profile.", user.Id);
            return Result.Success();
        }
    }
}
