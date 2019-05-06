using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public string GitHubUrl { get; set; }

        public int GitHubId { get; set; }

        public string GitHubToken { get; set; }

        public ICollection<Repository> Repositories { get; set; }
    }
}