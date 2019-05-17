using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Source
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Profile> Profiles { get; set; }
    }
}
