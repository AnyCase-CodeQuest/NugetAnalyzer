namespace NugetAnalyzer.Web.Infrastructure.Models
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
}
