using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Solutions;

namespace NugetAnalyzer.BLL.Models.Repositories
{
    public class RepositoryWithVersionReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SolutionWithVersionReport> Solutions { get; set; }
        public PackageVersionComparisonReport Report { get; set; }
    }
}
