using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models.Reports
{
    public class SolutionVersionReport : BaseVersionReport
    {
        public List<ProjectVersionReport> Projects { get; set; }
    }
}
