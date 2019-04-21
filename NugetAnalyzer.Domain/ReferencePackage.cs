using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class ReferencePackage : PackageBase
    {
        public ICollection<Package> CurrentPackages { get; set; }
    }
}