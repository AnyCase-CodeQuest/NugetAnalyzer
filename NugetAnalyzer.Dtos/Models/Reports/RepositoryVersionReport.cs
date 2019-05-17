using System.Collections.Generic;

namespace NugetAnalyzer.Dtos.Models.Reports
{
    public class RepositoryVersionReport : BaseVersionReport
    {
        public List<SolutionVersionReport> Solutions { get; set; }
    }
}
