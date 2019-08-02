using System;
using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Package : BaseEntity
    {
        public string Name { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public ICollection<PackageVersion> Versions { get; set; } = new List<PackageVersion>();
    }
}