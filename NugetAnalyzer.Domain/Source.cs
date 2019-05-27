using System.Collections.Generic;
using NugetAnalyzer.Domain.Enums;

namespace NugetAnalyzer.Domain
{
    public class Source : BaseEntity
    {
        public string Name { get; set; }

        public SourceType Type => (SourceType)Id;

        public ICollection<Profile> Profiles { get; set; }
    }
}
