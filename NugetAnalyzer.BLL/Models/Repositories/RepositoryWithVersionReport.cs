using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Solutions;

namespace NugetAnalyzer.BLL.Models.Repositories
{
    public class RepositoryWithVersionReport
    {
        public string Name { get; set; }
        public ICollection<SolutionWithVersionReport> Solutions { get; set; }
        public PackageVersionComparisonReport Report { get; set; }
    }
}
