using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Account;
using Socialify.Application.Interfaces;
using System.Linq;
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
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid login attempt.");
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
                return RedirectToAction("RegisterStep2");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterStep2()
        {
            var step1Data = TempData.Peek("RegisterModel") as string;
            if (string.IsNullOrEmpty(step1Data))
            {
                return RedirectToAction("RegisterStep1");
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
                    return RedirectToAction("Login", "Account");
                }

                result.Errors.ForEach(error => ModelState.AddModelError(string.Empty, error));
            }

            TempData.Keep("RegisterModel");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}