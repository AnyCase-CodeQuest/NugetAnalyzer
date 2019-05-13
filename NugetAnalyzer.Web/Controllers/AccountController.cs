using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public async Task<IActionResult> GitHubLogin(ProfileViewModel profile)
        {
            var userProfile = await userService.GetProfileByGitHubIdAsync(profile.GitHubId);

            if (userProfile != null)
            {
                await userService.UpdateUserAsync(profile);
                return RedirectToAction("Profile");
            }
            return RedirectToAction("UserCreationForm", profile);
        }

        [HttpGet]
        public IActionResult UserCreationForm(ProfileViewModel profile)
        {
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(ProfileViewModel profile)
        {
            await userService.CreateUserAsync(profile);

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGitHubId);
            var profile = await userService.GetProfileByGitHubIdAsync(userGitHubId);
            //countermeasures if user closed our site on profile registration form
            if (profile == null)
            {
                return RedirectToAction("Signout");
            }

            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}