using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models;
using NugetAnalyzer.Web.Models;

namespace NugetAnalyzer.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IProfileService profileService;
        private readonly ISourceService sourceService;

        public AccountController(IUserService userService, IProfileService profileService, ISourceService sourceService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
        }

        [HttpGet]
        public async Task<IActionResult> GitHubLogin(UserRegisterModel user)
        {
            var sourceList = await sourceService.GetSourceList();
            var sourceId = sourceList.First(sourceViewModel => sourceViewModel.Name == OAuthSourceNames.GitHubSourceName).Id;
            var gitHubId = user.ExternalId;

            var profile = await profileService.GetProfileBySourceIdAsync(sourceId, gitHubId);

            if (profile != null)
            {
                profile.AccessToken = user.AccessToken;
                await profileService.UpdateProfileAsync(profile);
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
            var sourceName = User.FindFirstValue(NugetAnalyzerClaimTypes.SourceNameClaimType);
            var stringExternalId = User.FindFirstValue(NugetAnalyzerClaimTypes.ExternalIdClaimType);
            int.TryParse(stringExternalId, out var externalId);
            //TODO: REFACTOR
            var sourceList = await sourceService.GetSourceList();
            var sourceId = sourceList.First(sourceViewModel => sourceViewModel.Name == sourceName).Id;
            var profile = await profileService.GetProfileBySourceIdAsync(sourceId, externalId);
            var userId = profile.UserId;
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