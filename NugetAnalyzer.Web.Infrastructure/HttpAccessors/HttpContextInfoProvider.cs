using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NugetAnalyzer.Domain.Enums;

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
                .FindFirstValue(Constants.NugetAnalyzerClaimTypes.UserNameClaimType);
        }

        public string GetAvatarUrl()
        {
            return httpContextAccessor
                .HttpContext
                .User
                .FindFirstValue(Constants.NugetAnalyzerClaimTypes.AvatarUrlClaimType);
        }

        public string GetExternalUrl()
        {
            return httpContextAccessor
                .HttpContext
                .User
                .FindFirstValue(Constants.NugetAnalyzerClaimTypes.GithubUrlClaimType);
        }

        public SourceType GetSource()
        {
            var type = httpContextAccessor
                .HttpContext
                .User
                .FindFirstValue(Constants.NugetAnalyzerClaimTypes.SourceClaimType);
            return Enum.Parse<SourceType>(type);
        }

        public int GetExternalId()
        {
            string stringId = httpContextAccessor
                .HttpContext
                .User
                .FindFirstValue(Constants.NugetAnalyzerClaimTypes.ExternalIdClaimType);

            return int.Parse(stringId);
        }
    }
}
