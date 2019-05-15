using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models;
using NugetAnalyzer.Web.Models;

namespace NugetAnalyzer.Web.Controllers
{
    public class GitHubAuthController : Controller
    {
        private const string AccessTokenName = "access_token";
        private readonly ISourceService sourceService;

        public GitHubAuthController(ISourceService sourceService)
        {
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/GitHubAuth/Authenticate" }, OAuthSourceNames.GitHubSourceName);
        }

        [HttpGet]
        public async Task<IActionResult> Authenticate()
        {
            var sourceEnum = await sourceService.GetSourceList();

            var sourceId = sourceEnum.First(sourceViewModel => sourceViewModel.Name == OAuthSourceNames.GitHubSourceName).Id;

            var accessToken = await HttpContext.GetTokenAsync(AccessTokenName);

            var user = new UserRegisterModel
            {
                UserName = User.FindFirstValue(NugetAnalyzerClaimTypes.UserNameClaimType),
                AvatarUrl = User.FindFirstValue(NugetAnalyzerClaimTypes.AvatarUrlClaimType),
                Url = User.FindFirstValue(NugetAnalyzerClaimTypes.GithubUrlClaimType),
                ExternalId = int.Parse(User.FindFirstValue(NugetAnalyzerClaimTypes.ExternalIdClaimType)),
                AccessToken = accessToken,
                SourceId = sourceId
            };

            return RedirectToAction("GitHubLogin", "Account", user);
        }
    }
}