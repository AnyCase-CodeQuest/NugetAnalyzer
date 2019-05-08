using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Projects;

namespace NugetAnalyzer.BLL.Models.Solutions
{
    public class SolutionWithVersionReport
    {
        public string Name { get; set; }
        public ICollection<ProjectWithVersionReport> Projects { get; set; }
        public PackageVersionComparisonReport Report { get; set; }
    }
}
