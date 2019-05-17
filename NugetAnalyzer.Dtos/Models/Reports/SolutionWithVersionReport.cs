using System.Collections.Generic;

namespace NugetAnalyzer.Dtos.Models.Reports
{
    public class SolutionWithVersionReport : BaseVersionReport
    {
        public List<ProjectWithVersionReport> Projects { get; set; }
    }
}
