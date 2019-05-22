using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Source : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Profile> Profiles { get; set; }
    }
}
