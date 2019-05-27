using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain.Enums;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.Web.Infrastructure;
using NugetAnalyzer.Web.Infrastructure.HttpAccessors;
using Octokit;

namespace NugetAnalyzer.Web.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IProfileService profileService;
        private readonly ISourceService sourceService;
        private readonly HttpContextInfoProvider httpContextInfoProvider;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IUserService userService,
            IProfileService profileService,
            ISourceService sourceService,
            HttpContextInfoProvider httpContextInfoProvider,
            ILogger<AccountController> logger)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            this.httpContextInfoProvider = httpContextInfoProvider ?? throw new ArgumentNullException(nameof(httpContextInfoProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GitHubLogin(UserRegisterModel user)
        {
            ProfileDTO profile = await profileService.GetProfileForUserAsync(user, SourceType.GitHub);
            if (profile != null)
            {
                return RedirectToAction("Profile");
            }
            return View("UserCreationForm", user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRegisterModel user)
        {
            logger.LogError("0000000000000000000000000000000000000\n000000000000000000000000000000000000\n00000000000000000000000000000000000000000");
            await userService.CreateUserAsync(user);
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int externalId = httpContextInfoProvider.GetExternalId();
            var source = httpContextInfoProvider.GetSource();

            int userId = await profileService.GetUserIdByExternalIdAsync(source, externalId);

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