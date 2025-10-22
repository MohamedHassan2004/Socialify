using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Account;
using Socialify.Application.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Socialify.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
                }
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Invalid login attempt. Please check your credentials and try again.";
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterStep1()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep1(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emailExists = await _authService.IsEmailExistsAsync(model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            TempData["RegisterModel"] = JsonSerializer.Serialize(model);
            return RedirectToAction(nameof(RegisterStep2));
        }

        [HttpGet]
        public IActionResult RegisterStep2()
        {
            var step1Data = TempData.Peek("RegisterModel") as string;
            if (string.IsNullOrEmpty(step1Data))
            {
                return RedirectToAction(nameof(RegisterStep1));
            }

            var registerModel = JsonSerializer.Deserialize<RegisterDto>(step1Data);
            ViewBag.RegisterEmail = registerModel?.Email;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep2(CompleteProfileDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("RegisterModel");
                return View(model);
            }

            var registerModelJson = TempData["RegisterModel"] as string;
            if (string.IsNullOrEmpty(registerModelJson))
            {
                TempData["ErrorMessage"] = "Registration session expired. Please start again.";
                return RedirectToAction(nameof(RegisterStep1));
            }

            var registerModel = JsonSerializer.Deserialize<RegisterDto>(registerModelJson);
            if (registerModel == null)
            {
                TempData["ErrorMessage"] = "An error occurred. Please start again.";
                return RedirectToAction(nameof(RegisterStep1));
            }

            var result = await _authService.RegisterAsync(registerModel, model);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Account created successfully! Please log in to continue.";
                return RedirectToAction(nameof(Login));
            }

            TempData["ErrorMessage"] = result.ErrorMessage ?? "Registration failed. Please try again.";

            if (result.Errors.Any(e => e.Contains("email", StringComparison.OrdinalIgnoreCase) ||
                                        e.Contains("password", StringComparison.OrdinalIgnoreCase)))
            {
                TempData["Step1Errors"] = JsonSerializer.Serialize(result.Errors);
                return RedirectToAction(nameof(RegisterStep1));
            }

            result.Errors.ForEach(error => ModelState.AddModelError(string.Empty, error));
            TempData.Keep("RegisterModel");
            return View(model);
        }

        [HttpGet]
        public IActionResult BackToStep1()
        {
            var step1Data = TempData.Peek("RegisterModel") as string;
            if (string.IsNullOrEmpty(step1Data))
            {
                return RedirectToAction(nameof(RegisterStep1));
            }

            var registerModel = JsonSerializer.Deserialize<RegisterDto>(step1Data);
            return View("RegisterStep1", registerModel);
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Json(false);

            var exists = await _authService.IsEmailExistsAsync(email);
            return Json(exists);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction(nameof(Login));
                }
                var result = await _authService.ChangePasswordAsync(model, userId);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Password changed successfully.";
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
                }
                TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred while changing the password.";
                result.Errors.ForEach(error => ModelState.AddModelError(string.Empty, error));
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        { 
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login));
            }
            var result = await _authService.DeleteAccountAsync(userId);
            if(result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Your account has been successfully deleted.";
                return RedirectToAction(nameof(Login));
            }
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred while deleting the account. Please try again.";
            return RedirectToAction(nameof(ProfileController.Settings), nameof(ProfileController).Replace("Controller", ""));
        }

    }
}