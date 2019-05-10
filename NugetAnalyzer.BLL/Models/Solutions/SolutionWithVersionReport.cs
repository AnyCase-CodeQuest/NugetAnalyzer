using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Projects;

namespace NugetAnalyzer.BLL.Models.Solutions
{
    public class SolutionWithVersionReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProjectWithVersionReport> Projects { get; set; }
        public PackageVersionComparisonReport Report { get; set; }
    }
}
