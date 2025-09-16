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
    [Authorize]
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IProfilePageService _profilePageService;

        public ProfileController(IProfileService profileService, IProfilePageService profilePageService)
        {
            _profileService = profileService;
            _profilePageService = profilePageService;
        }

        private string GetCurrentUserId() 
            => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("MyProfile")]
        public async Task<IActionResult> MyProfile()
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var profileResult = await _profilePageService.GetProfilePageAsync(currentUserId, currentUserId);
            if (!profileResult.IsSuccess)
            {
                return View("Error");
            }

            return View("Index", profileResult.Data);
        }

        [HttpGet("{targetUserId}")]
        public async Task<IActionResult> Profile(string targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var profileResult = await _profilePageService.GetProfilePageAsync(targetUserId, currentUserId);
            if (!profileResult.IsSuccess)
            {
                return View("Error");
            }

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
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }
            var profileInfoResult = await _profileService.GetProfileInfoAsync(currentUserId);
            if (!profileInfoResult.IsSuccess)
            {
                return View("Error");
            }
            return View(profileInfoResult.Data);
        }

        [HttpPost("UpdateProfileInfo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileInfoAsync(UpdateProfileInfoDto updateProfileInfoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateProfileInfoDto);
            }
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }
            var result = await _profileService.UpdateProfileInfoAsync(currentUserId, updateProfileInfoDto);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(MyProfile));
            }
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to update profile info. Please try again.");
            return View(updateProfileInfoDto);
        }

        [HttpPost("RemoveProfilePic")]
        public async Task<IActionResult> RemoveProfilePicAsync()
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var result = await _profileService.RemoveProfilePictureAsync(currentUserId);
            if (result.IsSuccess)
            {
                return NoContent();
            }

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
            if (!ModelState.IsValid)
            {
                return View(patchProfilePicDto);
            }

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var result = await _profileService.UpdateProfilePictureAsync(currentUserId, patchProfilePicDto);
            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(MyProfile));
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to update profile picture. Please try again.");
            return View(patchProfilePicDto);
        }

    }
}