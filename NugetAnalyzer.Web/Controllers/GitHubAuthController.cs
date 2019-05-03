using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Models;

namespace NugetAnalyzer.Web.Controllers
{
    public class GitHubAuthController : Controller
    {
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/GitHubAuth/Authenticate" }, "GitHub");
        }

        public IActionResult Authenticate()
        {
            var profile = new Profile
            {
                UserName = User.FindFirstValue("urn:github:login"),
                GitHubUrl = User.FindFirstValue("urn:github:url"),
                GitHubId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                AvatarUrl = User.FindFirstValue("urn:github:avatar")
            };

            return RedirectToAction("UserGitHubLogin", "Account", profile);
        }
    }
}