using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NugetAnalyzer.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult UserGitHubLogin(Profile profile)
        {
            Profile userProfile = userService.GetProfileByGitHubId(profile.GitHubId);
            if (userProfile != null)
            {
                return View("Profile", userProfile);
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
            await userService.CreateUserAsync(profile);

            var userProfile = userService.GetProfileByGitHubId(profile.GitHubId);
            return View("Profile", userProfile);
        }

        public IActionResult Profile()
        {
            var userGitHubId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = userService.GetProfileByGitHubId(userGitHubId);
            return View("Profile", profile);
        }
    }
}