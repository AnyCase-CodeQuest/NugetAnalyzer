using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models.Reports
{
    public class RepositoryVersionReport : BaseVersionReport
    {
        public List<SolutionVersionReport> Solutions { get; set; }
    }
}
