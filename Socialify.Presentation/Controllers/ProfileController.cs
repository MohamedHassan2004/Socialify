using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Domain.Entities;
using System.Threading.Tasks;

namespace Socialify.Presentation.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Complete()
        {
            ViewBag.HideNavbar = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(CompleteProfileDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.GetCurrentUserAsync();
                if (!result.IsSuccess || result.Data == null)
                {
                    return NotFound();
                }

                var user = result.Data;
                await _userService.UpdateUserAsync(user, model);
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
    }
}
