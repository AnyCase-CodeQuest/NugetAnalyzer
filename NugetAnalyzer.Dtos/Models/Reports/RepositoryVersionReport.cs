using System.Collections.Generic;

namespace NugetAnalyzer.Dtos.Models.Reports
{
    public class RepositoryWithVersionReport : BaseVersionReport
    {
        public List<SolutionWithVersionReport> Solutions { get; set; }
    }
}
