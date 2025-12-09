using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Account;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.Mappers;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Domain.Events;
using System.Security.Claims;

namespace Socialify.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthService> logger,
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
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

        public async Task<Result> ChangePasswordAsync(ChangePasswordDto changePasswordDto, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure("User not found.");
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

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

        public async Task<Result> DeleteAccountAsync(string userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure("User not found.");
                }

                await _mediator.Publish(new UserDeletingEvent(userId));
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    await _unitOfWork.CommitTransactionAsync();
                    await _signInManager.SignOutAsync();
                    return Result.Success();
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error occurred during account deletion for user {UserId}", userId);
                return Result.Failure("An error occurred during account deletion.");
            }
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
