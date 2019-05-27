using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain.Enums;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.Web.Infrastructure;
using NugetAnalyzer.Web.Infrastructure.HttpAccessors;

namespace NugetAnalyzer.Web.Controllers
{
	public class GitHubAuthController : Controller
	{
		private readonly ISourceService sourceService;
		private readonly HttpContextInfoProvider httpContextInfoProvider;
        private const string redirectUri = "/GitHubAuth/Authenticate";


        public GitHubAuthController(ISourceService sourceService, HttpContextInfoProvider httpContextInfoProvider)
		{
			this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
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