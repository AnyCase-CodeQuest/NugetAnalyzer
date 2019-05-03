using System;
using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Package
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public ICollection<PackageVersion> Versions { get; set; }
    }
}