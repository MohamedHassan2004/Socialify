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

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return Result<UserDto>.Failure("Invalid email or password.");
                }

                if (!user.IsActive)
                {
                    return Result<UserDto>.Failure("Account is deactivated.");
                }

                var Result = await _signInManager.CheckPasswordSignInAsync(
                                            user, loginDto.Password, lockoutOnFailure: true);

                if (Result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim("FullName", $"{user.FirstName} {user.LastName}".Trim()),
                        new Claim("ProfilePic", user.ProfilePicUrl)
                    };

                    var authProps = new AuthenticationProperties
                    {
                        IsPersistent = loginDto.RememberMe,
                        ExpiresUtc = loginDto.RememberMe
                            ? DateTimeOffset.UtcNow.AddDays(14)
                            : DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    await _signInManager.SignInWithClaimsAsync(user, authProps, claims);

                    var dto = _mapper.Map<UserDto>(user);
                    _logger.LogInformation("User {Email} logged in successfully.", user.Email);
                    return Result<UserDto>.Success(dto);
                }

                if (Result.IsLockedOut)
                    return Result<UserDto>.Failure("Account is locked out.");

                return Result<UserDto>.Failure("Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Email}", loginDto.Email);
                return Result<UserDto>.Failure("An error occurred during login.");
            }
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return Result<UserDto>.Failure("Email already exists.");
                }

                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    var userDto = _mapper.Map<UserDto>(user);
                    _logger.LogInformation("User {Email} registered successfully.", user.Email);
                    return Result<UserDto>.Success(userDto);
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<UserDto>.Failure(errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for {Email}", registerDto.Email);
                return Result<UserDto>.Failure("An error occurred during registration.");
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

        public async Task<Result<UserDto>> GetCurrentUserAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_signInManager.Context.User);
                if (user == null)
                {
                    return Result<UserDto>.Failure("User not found.");
                }

                var userDto = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting current user");
                return Result<UserDto>.Failure("An error occurred while getting user information.");
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

    }
}

