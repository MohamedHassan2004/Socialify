using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.Services;
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
        private readonly int PageSize = 5;


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
            var profileResult = await _profilePageService.GetProfilePageAsync(currentUserId, currentUserId, PageSize);
            if (!profileResult.IsSuccess)
                return HandleServiceError(profileResult, nameof(MyProfile));

            return View("Index", profileResult.Data);
        }

        [HttpGet("{targetUserId}")]
        public async Task<IActionResult> Profile(string targetUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
                return HandleUnauthorizedAccess(nameof(Profile));

            var profileResult = await _profilePageService.GetProfilePageAsync(targetUserId, currentUserId, PageSize);
            if (!profileResult.IsSuccess)
                return HandleServiceError(profileResult, nameof(Profile));

            return View("Index", profileResult.Data);
        }

        [HttpGet("Settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet("UpdateProfileInfo")]
        public async Task<IActionResult> UpdateProfileInfo()
        {
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

        [HttpPost("UpdateProfileInfo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileInfoAsync(UpdateProfileInfoDto updateProfileInfoDto)
        {
            if (!ValidateModelAndLogErrors(updateProfileInfoDto, nameof(UpdateProfileInfoAsync)))
                return View(updateProfileInfoDto);

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

        [HttpPost("RemoveProfilePic")]
        public async Task<IActionResult> RemoveProfilePicAsync()
        {
            var result = await _profileService.RemoveProfilePictureAsync(currentUserId);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Profile picture removed successfully for user {UserId}", currentUserId);
                return Ok();
            }

            _logger.LogError("Failed to remove profile picture for user {UserId}: {ErrorMessage}", currentUserId, result.ErrorMessage);
            return BadRequest(result.ErrorMessage ?? "Failed to remove profile picture. Please try again.");
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
            if (!ValidateModelAndLogErrors(patchProfilePicDto, nameof(UpdateProfilePicAsync)))
                return View(patchProfilePicDto);

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
    }
}