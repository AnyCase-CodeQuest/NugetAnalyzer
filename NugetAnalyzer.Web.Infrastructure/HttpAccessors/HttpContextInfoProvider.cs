using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NugetAnalyzer.Web.Infrastructure.Models;

namespace NugetAnalyzer.Web.Infrastructure.HttpAccessors
{
	public class HttpContextInfoProvider
	{

		private const string AccessTokenName = "access_token";

		private readonly IHttpContextAccessor httpContextAccessor;

		public HttpContextInfoProvider(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public async Task<string> GetAccessTokenAsync()
		{
			return await httpContextAccessor
				.HttpContext
				.GetTokenAsync(AccessTokenName);
		}

		public string GetUsername()
		{
			return httpContextAccessor
				.HttpContext
				.User
				.FindFirstValue(NugetAnalyzerClaimTypes.UserNameClaimType);
		}

		public string GetAvatarUrl()
		{
			return httpContextAccessor
				.HttpContext
				.User
				.FindFirstValue(NugetAnalyzerClaimTypes.AvatarUrlClaimType);
		}

		public string GetExternalUrl()
		{
			return httpContextAccessor
				.HttpContext
				.User
				.FindFirstValue(NugetAnalyzerClaimTypes.GithubUrlClaimType);
		}

		public string GetSourceName()
		{
			return httpContextAccessor
				.HttpContext
				.User
				.FindFirstValue(NugetAnalyzerClaimTypes.SourceNameClaimType);
		}

		public int GetExternalId()
		{
			var stringId = httpContextAccessor
				.HttpContext
				.User
				.FindFirstValue(NugetAnalyzerClaimTypes.ExternalIdClaimType);

			return int.Parse(stringId);
		}
	}
}
