using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.Web.Infrastructure;
using NugetAnalyzer.Web.Infrastructure.HttpAccessors;

namespace NugetAnalyzer.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IProfileService profileService;
        private readonly ISourceService sourceService;
        private readonly HttpContextInfoProvider httpContextInfoProvider;

        public AccountController(
            IUserService userService,
            IProfileService profileService,
            ISourceService sourceService,
            HttpContextInfoProvider httpContextInfoProvider)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            this.httpContextInfoProvider = httpContextInfoProvider ?? throw new ArgumentNullException(nameof(httpContextInfoProvider));
        }

        [HttpGet]
        public async Task<IActionResult> GitHubLogin(UserRegisterModel user)
        {
            int sourceId = await sourceService.GetSourceIdByName(Constants.OAuthSourceNames.GitHubSourceName);
            ProfileDTO profile = await profileService.GetProfileForUserAsync(user, sourceId);
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
            string sourceName = httpContextInfoProvider.GetSourceName();
            int externalId = httpContextInfoProvider.GetExternalId();

            int sourceId = await sourceService.GetSourceIdByName(sourceName);

            int userId = await profileService.GetUserIdByExternalIdAsync(sourceId, externalId);

            UserDTO user = await userService.GetUserByIdAsync(userId);

            //countermeasures if user closed our site on profile registration form
            if (user == null)
            {
                return RedirectToAction("SignOut");
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}