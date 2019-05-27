using System.Security.Claims;
using NugetAnalyzer.Domain.Enums;

namespace NugetAnalyzer.Web.Infrastructure
{
    public static class Constants
    {
        /// <summary>
        /// Defines constants for GitHub OAuth scopes.
        /// </summary>
        public static class GitHubScopes
        {
            /// <summary>
            /// Grants full access to private and public repositories.
            /// That includes read/write access to code, commit statuses, repository and organization projects, invitations, collaborators, adding team memberships,
            /// and deployment statuses for public and private repositories and organizations. Also grants ability to manage user projects.
            /// </summary>
            public const string Repo = "repo";
        }

        /// <summary>
        /// Defines constants for GitHub user information keys.
        /// </summary>
        public static class GitHubUserClaimTypes
        {
            public const string Id = "id";
            public const string UserName = "login";
            public const string Url = "html_url";
            public const string AvatarUrl = "avatar_url";
        }

        /// <summary>
        /// Defines constants for custom user claims.
        /// </summary>
        public static class NugetAnalyzerClaimTypes
        {
            public const string UserNameClaimType = "login";
            public const string GithubUrlClaimType = "url";
            public const string AvatarUrlClaimType = "avatarUrl";
            public const string SourceClaimType = "source";
            public const string ExternalIdClaimType = ClaimTypes.NameIdentifier;
            public const string GitHubSourceName = "GitHub";
        }

        public static class OAuthSourceNames
        {
            public const SourceType GitHubSourceType = SourceType.GitHub;
        }
    }
}
