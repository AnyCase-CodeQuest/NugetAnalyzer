using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.Web.Controllers
{
    public class GitHubAuthController : Controller
    {
        private const string UserNameClaimType = "urn:github:login";
        private const string GithubUrlClaimType = "urn:github:url";
        private const string AvatarUrlClaimType = "urn:github:avatar";
        private const string GithubIdClaimType = ClaimTypes.NameIdentifier;
        private const string AccessTokenName = "access_token";
        private const string gitHubSourceName = "GitHub";

        private readonly ISourceService sourceService;

        public GitHubAuthController(ISourceService sourceService)
        {
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/GitHubAuth/Authenticate" }, gitHubSourceName);
        }

        [HttpGet]
        public async Task<IActionResult> Authenticate()
        {
            var sourceEnum = await sourceService.GetSourceList();

            var sourceId = sourceEnum.First(p => p.Name == gitHubSourceName).Id;

            var accessToken = await HttpContext.GetTokenAsync(AccessTokenName);

            var user = new UserRegisterModel
            {
                UserName = User.FindFirstValue(UserNameClaimType),
                AvatarUrl = User.FindFirstValue(AvatarUrlClaimType),
                Url = User.FindFirstValue(GithubUrlClaimType),
                IdOnSource = int.Parse(User.FindFirstValue(GithubIdClaimType)),
                AccessToken = accessToken,
                SourceId = sourceId
            };

            return RedirectToAction("GitHubLogin", "Account", user);
        }
    }
}