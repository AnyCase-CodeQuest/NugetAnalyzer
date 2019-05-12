using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;

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

        public async Task<IActionResult> Profile()
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            // Culture contains the information of the requested culture
            var culture = rqf.RequestCulture.Culture;
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGitHubId);
            var profile = await userService.GetProfileByGitHubIdAsync(userGitHubId);
            //countermeasures if user closed our site on profile registration form
            if (profile == null)
            {
                return RedirectToAction("Signout");
            }

            return View(profile);
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}