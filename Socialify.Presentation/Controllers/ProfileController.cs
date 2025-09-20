using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.Services_Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Socialify.Presentation.Controllers
{
    [Route("[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;
        private readonly IProfilePageService _profilePageService;

        public ProfileController(
            IProfileService profileService, 
            IProfilePageService profilePageService,
            ILogger<ProfileController> logger) : base(logger)
        {
            _profileService = profileService;
            _profilePageService = profilePageService;
        }

        [HttpGet("MyProfile")]
        public async Task<IActionResult> MyProfile()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                    return HandleUnauthorizedAccess(nameof(MyProfile));

                var profileResult = await _profilePageService.GetProfilePageAsync(currentUserId, currentUserId);
                if (!profileResult.IsSuccess)
                    return HandleServiceError(profileResult, nameof(MyProfile));

                return View("Index", profileResult.Data);
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, nameof(MyProfile));
            }
        }

        [HttpGet("{targetUserId}")]
        public async Task<IActionResult> Profile(string targetUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(targetUserId))
                {
                    _logger.LogWarning("Invalid targetUserId provided: {TargetUserId}", targetUserId);
                    TempData["ErrorMessage"] = "Invalid profile requested.";
                    return RedirectToAction(nameof(MyProfile));
                }

                var currentUserId = GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                    return HandleUnauthorizedAccess(nameof(Profile));

                var profileResult = await _profilePageService.GetProfilePageAsync(targetUserId, currentUserId);
                if (!profileResult.IsSuccess)
                    return HandleServiceError(profileResult, nameof(Profile));

                return View("Index", profileResult.Data);
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, nameof(Profile));
            }
        }

        [HttpGet("Settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet("UpdateProfileInfo")]
        public async Task<IActionResult> UpdateProfileInfo()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                    return HandleUnauthorizedAccess(nameof(UpdateProfileInfo));

                var profileInfoResult = await _profileService.GetProfileInfoAsync(currentUserId);
                if (!profileInfoResult.IsSuccess)
                {
                    _logger.LogError("Failed to load profile info for user {UserId}: {ErrorMessage}", 
                        currentUserId, profileInfoResult.ErrorMessage);
                    TempData["ErrorMessage"] = profileInfoResult.ErrorMessage ?? "An error occurred while loading profile information. Please try again.";
                    return RedirectToAction(nameof(MyProfile));
                }

                return View(profileInfoResult.Data);
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, nameof(UpdateProfileInfo));
            }
        }

        [HttpPost("UpdateProfileInfo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileInfoAsync(UpdateProfileInfoDto updateProfileInfoDto)
        {
            try
            {
                if (!ValidateModelAndLogErrors(updateProfileInfoDto, nameof(UpdateProfileInfoAsync)))
                    return View(updateProfileInfoDto);

                var currentUserId = GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                    return HandleUnauthorizedAccess(nameof(UpdateProfileInfoAsync));

                var result = await _profileService.UpdateProfileInfoAsync(currentUserId, updateProfileInfoDto);
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Profile info updated successfully for user {UserId}", currentUserId);
                    TempData["SuccessMessage"] = "Profile information updated successfully!";
                    return RedirectToAction(nameof(MyProfile));
                }

                _logger.LogError("Failed to update profile info for user {UserId}: {ErrorMessage}", 
                    currentUserId, result.ErrorMessage);
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to update profile info. Please try again.";
                return View(updateProfileInfoDto);
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, nameof(UpdateProfileInfoAsync));
            }
        }

        [HttpPost("RemoveProfilePic")]
        public async Task<IActionResult> RemoveProfilePicAsync()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                    return HandleUnauthorizedAccess(nameof(RemoveProfilePicAsync));

                var result = await _profileService.RemoveProfilePictureAsync(currentUserId);
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Profile picture removed successfully for user {UserId}", currentUserId);
                    return NoContent();
                }

                _logger.LogError("Failed to remove profile picture for user {UserId}: {ErrorMessage}", 
                    currentUserId, result.ErrorMessage);
                return BadRequest(result.ErrorMessage ?? "Failed to remove profile picture. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error removing profile picture for user {UserId}", GetCurrentUserId());
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        [HttpGet("UpdateProfilePic")]
        public IActionResult UpdateProfilePic()
        {
            return View();
        }

        [HttpPost("UpdateProfilePic")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfilePicAsync(PatchProfilePicDto patchProfilePicDto)
        {
            try
            {
                if (!ValidateModelAndLogErrors(patchProfilePicDto, nameof(UpdateProfilePicAsync)))
                    return View(patchProfilePicDto);

                var currentUserId = GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                    return HandleUnauthorizedAccess(nameof(UpdateProfilePicAsync));

                var result = await _profileService.UpdateProfilePictureAsync(currentUserId, patchProfilePicDto);
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Profile picture updated successfully for user {UserId}", currentUserId);
                    TempData["SuccessMessage"] = "Profile picture updated successfully!";
                    return RedirectToAction(nameof(MyProfile));
                }

                _logger.LogError("Failed to update profile picture for user {UserId}: {ErrorMessage}", 
                    currentUserId, result.ErrorMessage);
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to update profile picture. Please try again.";
                return View(patchProfilePicDto);
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, nameof(UpdateProfilePicAsync));
            }
        }
    }
}