using System.Collections.Generic;
using System.Reflection.Metadata;

namespace NugetAnalyzer.Domain
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Repository> Repositories { get; set; }
    }
}
