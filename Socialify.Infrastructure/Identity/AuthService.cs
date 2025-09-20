using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Socialify.Application.Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System.Security.Claims;
using Socialify.Application.DTOs.Account;
using Socialify.Application.DTOs.Profile;

namespace Socialify.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return Result.Failure("Invalid email or password.");
                }

                if (!user.IsActive)
                {
                    return Result.Failure("Account is deactivated.");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(
                                            user, loginDto.Password, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim("FullName", $"{user.FirstName} {user.LastName}".Trim()),
                    };

                    var authProps = new AuthenticationProperties
                    {
                        IsPersistent = loginDto.RememberMe,
                        ExpiresUtc = loginDto.RememberMe
                            ? DateTimeOffset.UtcNow.AddDays(14)
                            : DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    await _signInManager.SignInWithClaimsAsync(user, authProps, claims);

                    _logger.LogInformation("User {Email} logged in successfully.", user.Email);
                    return Result.Success();
                }

                if (result.IsLockedOut)
                    return Result.Failure("Account is locked out.");

                return Result.Failure("Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Email}", loginDto.Email);
                return Result.Failure("An error occurred during login.");
            }
        }

        public async Task<Result> RegisterAsync(RegisterDto registerDto, CompleteProfileDto completeProfileDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return Result.Failure("Email already exists.");
                }

                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName.Trim(),
                    LastName = registerDto.LastName.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    PhoneNumber = completeProfileDto.PhoneNumber,
                    Gender = completeProfileDto.Gender,
                    Bio = completeProfileDto.Bio,
                    BirthDate = completeProfileDto.BirthDate
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    _logger.LogInformation("User {Email} registered successfully.", user.Email);
                    return Result.Success();
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for {Email}", registerDto.Email);
                return Result.Failure("An error occurred during registration.");
            }
        }

        public async Task<Result> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout");
                return Result.Failure("An error occurred during logout.");
            }
        }


        public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure("User not found.");
                }

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (result.Succeeded)
                {
                    return Result.Success();
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password change for user {UserId}", userId);
                return Result.Failure("An error occurred during password change.");
            }
        }

        public async Task<Result<ProfileDto>> GetCurrentUserAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_signInManager.Context.User);
                if (user == null)
                {
                    return Result<ProfileDto>.Failure("User not found.");
                }
                var dto = _mapper.Map<ProfileDto>(user);
                return Result<ProfileDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching current user");
                return Result<ProfileDto>.Failure("An error occurred while fetching user data.");
            }

        }

        public async Task<Result> DeleteAccountAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure("User not found.");
                }
                var result = _userManager.DeleteAsync(user);
                if (result.Result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return Result.Success();
                }
                var errors = result.Result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during account deletion for user {UserId}", userId);
                return Result.Failure("An error occurred during account deletion.");
            }
        }
    }
}
