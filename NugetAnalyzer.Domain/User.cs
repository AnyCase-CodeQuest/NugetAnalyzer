using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public ICollection<Profile> Profiles { get; set; }

        public ICollection<Repository> Repositories { get; set; }
    }
}