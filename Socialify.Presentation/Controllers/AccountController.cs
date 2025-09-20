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
                    TempData["SuccessMessage"] = "Welcome back! You have successfully logged in.";
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
        public IActionResult RegisterStep1(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                TempData["RegisterModel"] = JsonSerializer.Serialize(model);
                return RedirectToAction(nameof(RegisterStep2));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterStep2()
        {
            var step1Data = TempData.Peek("RegisterModel") as string;
            if (string.IsNullOrEmpty(step1Data))
            {
                return RedirectToAction(nameof(RegisterStep1));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStep2(CompleteProfileDto model)
        {
            if (ModelState.IsValid)
            {
                var registerModelJson = TempData["RegisterModel"] as string;
                if (string.IsNullOrEmpty(registerModelJson))
                {
                    ModelState.AddModelError(string.Empty, "Registration session expired. Please register again.");
                    return View(model);
                }

                var registerModel = JsonSerializer.Deserialize<RegisterDto>(registerModelJson);
                if (registerModel == null)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred. Please register again.");
                    return View(model);
                }

                var result = await _authService.RegisterAsync(registerModel, model);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Account created successfully! Please log in to continue.";
                    return RedirectToAction(nameof(Login));
                }

                TempData["ErrorMessage"] = result.ErrorMessage ?? "Registration failed. Please try again.";
                result.Errors.ForEach(error => ModelState.AddModelError(string.Empty, error));
            }

            TempData.Keep("RegisterModel");
            return View(model);
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
                var result = await _authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
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