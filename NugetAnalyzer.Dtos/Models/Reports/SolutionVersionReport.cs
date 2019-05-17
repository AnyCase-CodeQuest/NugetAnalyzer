using System.Collections.Generic;

namespace NugetAnalyzer.Dtos.Models.Reports
{
    public class SolutionVersionReport : BaseVersionReport
    {
        public List<ProjectVersionReport> Projects { get; set; }
    }
}
