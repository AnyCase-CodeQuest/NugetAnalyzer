using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models;
using NugetAnalyzer.Web.HttpAccessors;
using NugetAnalyzer.Web.Models;

namespace NugetAnalyzer.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IProfileService profileService;
        private readonly ISourceService sourceService;
        private readonly HttpContextInfoProvider httpContextInfoProvider;

        public AccountController(IUserService userService, IProfileService profileService, ISourceService sourceService, HttpContextInfoProvider httpContextInfoProvider)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            this.httpContextInfoProvider = httpContextInfoProvider ?? throw new ArgumentNullException(nameof(httpContextInfoProvider));
        }

        [HttpGet]
        public async Task<IActionResult> GitHubLogin(UserRegisterModel user)
        {
            var sourceId = await sourceService.GetSourceIdByName(OAuthSourceNames.GitHubSourceName);
            var profile = await profileService.GetProfileForUserAsync(user, sourceId);
            if (profile != null)
            {
                return RedirectToAction("Profile");
            }
            return View("UserCreationForm", user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRegisterModel user)
        {
            await userService.CreateUserAsync(user);
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var sourceName = httpContextInfoProvider.GetSourceName();
            var externalId = httpContextInfoProvider.GetExternalId();

            var sourceId = await sourceService.GetSourceIdByName(sourceName);

            var userId = await profileService.GetUserIdByExternalIdAsync(sourceId, externalId);

            var user = await userService.GetUserByIdAsync(userId);

            //countermeasures if user closed our site on profile registration form
            if (user == null)
            {
                return RedirectToAction("Signout");
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}