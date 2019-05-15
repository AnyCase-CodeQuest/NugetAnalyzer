using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Projects;

namespace NugetAnalyzer.BLL.Models.Solutions
{
    public class SolutionWithVersionReport : ModelWithReport
    {
        public List<ProjectWithVersionReport> Projects { get; set; }
    }
}
