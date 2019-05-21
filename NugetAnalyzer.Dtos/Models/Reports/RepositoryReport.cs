using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models.Reports
{
    public class RepositoryReport : BaseVersionReport
    {
        public List<SolutionReport> Solutions { get; set; }
    }
}
