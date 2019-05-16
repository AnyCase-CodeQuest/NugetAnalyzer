using System.Collections.Generic;
using NugetAnalyzer.Dtos.Models.Solutions;

namespace NugetAnalyzer.Dtos.Models.Repositories
{
    public class RepositoryWithVersionReport : ModelWithReport
    {
        public List<SolutionWithVersionReport> Solutions { get; set; }
    }
}
