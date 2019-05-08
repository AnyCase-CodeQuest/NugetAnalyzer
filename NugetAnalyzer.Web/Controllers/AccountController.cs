using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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
            var userProfile = userService.GetProfileByGitHubId(profile.GitHubId);

            if (userProfile != null)
            {
                var gitHubToken = HttpContext.GetTokenAsync("access_token").Result;
                await userService.UpdateGitHubToken(profile.GitHubId, gitHubToken);
                return RedirectToAction("Profile", userProfile);
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

            profile = userService.GetProfileByGitHubId(profile.GitHubId);
            return RedirectToAction("Profile", profile);
        }

        public IActionResult Profile(Profile profile)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //countermeasures if user closed our site on profile registration form
            if (profile.UserName == null)
            {
                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userGitHubId);
                profile = userService.GetProfileByGitHubId(userGitHubId);
                if (profile == null)
                {
                    return RedirectToAction("Signout");
                }
            }

            return View("Profile", profile);
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}