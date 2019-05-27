using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.Domain.Enums;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.Web.Infrastructure;
using NugetAnalyzer.Web.Infrastructure.HttpAccessors;

namespace NugetAnalyzer.Web.Controllers
{
	public class GitHubAuthController : Controller
	{
		private readonly HttpContextInfoProvider httpContextInfoProvider;
        private const string redirectUri = "/GitHubAuth/Authenticate";


        public GitHubAuthController(HttpContextInfoProvider httpContextInfoProvider)
		{
			this.httpContextInfoProvider = httpContextInfoProvider ?? throw new ArgumentNullException(nameof(httpContextInfoProvider));
		}

		[HttpGet]
		public IActionResult Login()
		{
			return Challenge(
				new AuthenticationProperties { RedirectUri = redirectUri },
				Constants.OAuthSourceNames.GitHubSourceType.ToString());
		}

		[HttpGet]
		public async Task<IActionResult> Authenticate()
		{
            string accessToken = await httpContextInfoProvider.GetAccessTokenAsync();

			var user = new UserRegisterModel
			{
				UserName = httpContextInfoProvider.GetUsername(),
				AvatarUrl = httpContextInfoProvider.GetAvatarUrl(),
				Url = httpContextInfoProvider.GetExternalUrl(),
				ExternalId = httpContextInfoProvider.GetExternalId(),
				AccessToken = accessToken,
				SourceId = (int)SourceType.GitHub
			};

			return RedirectToAction("GitHubLogin", "Account", user);
		}
	}
}