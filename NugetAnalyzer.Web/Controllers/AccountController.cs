using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;

namespace NugetAnalyzer.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> GitHubLogin(Profile profile)
        {
            var userProfile = userService.GetProfileByGitHubIdAsync(profile.GitHubId);

            if (userProfile != null)
            {
                var gitHubToken = await HttpContext.GetTokenAsync("access_token");
                await userService.UpdateGitHubTokenAsync(profile.GitHubId, gitHubToken);
                return RedirectToAction("Profile");
            }

            return RedirectToAction("UserCreationForm", profile);
        }

        [HttpGet]
        public IActionResult UserCreationForm(Profile profile)
        {
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(Profile profile)
        {
            var gitHubToken = await HttpContext.GetTokenAsync("access_token");
            await userService.CreateUserAsync(profile, gitHubToken);

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

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