using System.Security.Claims;

namespace NugetAnalyzer.Web.Models
{
    /// <summary>
    /// Defines constants for custom user claims.
    /// </summary>
    public static class NugetAnalyzerClaimTypes
    {
        public const string UserNameClaimType = "login";
        public const string GithubUrlClaimType = "url";
        public const string AvatarUrlClaimType = "avatarUrl";
        public const string SourceNameClaimType = "source";
        public const string ExternalIdClaimType = ClaimTypes.NameIdentifier;
        public const string GitHubSourceName = "GitHub";
    }
}
