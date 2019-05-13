using System;
using System.Linq;
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
        private const string AvatarUrlClaimType = "avatarUrl";
        private const string AuthenticationTypeScheme = "Cookies";
        private const string GitHubSourceName = "GitHub";
        private const string UserIdClaimType = "userId";

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
            var sourceId = sourceList.First(p => p.Name == GitHubSourceName).Id;
            var gitHubId = user.IdOnSource;

            var profile = await profileService.GetProfileBySourceIdAsync(sourceId, gitHubId);

            if (profile != null)
            {
                profile.AccessToken = user.AccessToken;
                await profileService.UpdateProfileAsync(profile);
                var currentUser = await userService.GetUserByIdAsync(profile.UserId);
                await ReloginAsync(currentUser);
                return RedirectToAction("Profile");
            }
            return View("UserCreationForm", user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRegisterModel user)
        {
            var createdUser = await userService.CreateUserAsync(user);
            await ReloginAsync(createdUser);
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var stringUserId = User.FindFirstValue(UserIdClaimType);
            int.TryParse(stringUserId, out var userId);
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

        private async Task ReloginAsync(UserViewModel user)
        {
            await HttpContext.SignOutAsync();
            var claimsIdentity = new ClaimsIdentity(AuthenticationTypeScheme);
            claimsIdentity.AddClaim(new Claim(UserIdClaimType, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(AvatarUrlClaimType, user.AvatarUrl));
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(AuthenticationTypeScheme, claimsPrincipal);
        }
    }
}