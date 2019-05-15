using System.Collections.Generic;
using NugetAnalyzer.Dtos.Models.Projects;

namespace NugetAnalyzer.Dtos.Models.Solutions
{
    public class SolutionWithVersionReport : ModelWithReport
    {
        public List<ProjectWithVersionReport> Projects { get; set; }
    }
}
