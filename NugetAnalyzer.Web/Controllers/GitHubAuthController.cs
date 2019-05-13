using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.Web.Controllers
{
    public class GitHubAuthController : Controller
    {
        private const string UserNameClaimType = "urn:github:login";
        private const string GithubUrlClaimType = "urn:github:url";
        private const string AvatarUrlClaimType = "urn:github:avatar";
        private const string GithubIdClaimType = ClaimTypes.NameIdentifier;

        [HttpGet]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/GitHubAuth/Authenticate" }, "GitHub");
        }

        [HttpGet]
        public async Task<IActionResult> Authenticate()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var profile = new ProfileViewModel
            {
                UserName = User.FindFirstValue(UserNameClaimType),
                GitHubUrl = User.FindFirstValue(GithubUrlClaimType),
                GitHubId = int.Parse(User.FindFirstValue(GithubIdClaimType)),
                AvatarUrl = User.FindFirstValue(AvatarUrlClaimType),
                AccessToken = accessToken
            };
            return RedirectToAction("GitHubLogin", "Account", profile);
        }
    }
}