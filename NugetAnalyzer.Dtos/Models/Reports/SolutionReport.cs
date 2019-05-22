using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models.Reports
{
    public class SolutionReport : BaseVersionReport
    {
        public List<ProjectReport> Projects { get; set; }
    }
}
