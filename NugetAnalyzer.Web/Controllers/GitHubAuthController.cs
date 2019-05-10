using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Models;

namespace NugetAnalyzer.Web.Controllers
{
    public class GitHubAuthController : Controller
    {
        private const string userName = "urn:github:login";
        private const string githubUrl = "urn:github:url";
        private const string avatarUrl = "urn:github:avatar";

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/GitHubAuth/Authenticate" }, "GitHub");
        }

        public IActionResult Authenticate()
        {
            var profile = new Profile
            {
                UserName = User.FindFirstValue(userName),
                GitHubUrl = User.FindFirstValue(githubUrl),
                GitHubId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                AvatarUrl = User.FindFirstValue(avatarUrl)
            };
            return RedirectToAction("GitHubLogin", "Account", profile);
        }
    }
}