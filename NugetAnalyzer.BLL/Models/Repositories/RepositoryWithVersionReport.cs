using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Solutions;

namespace NugetAnalyzer.BLL.Models.Repositories
{
    public class RepositoryWithVersionReport : ModelWithReport
    {
        public List<SolutionWithVersionReport> Solutions { get; set; }
    }
}
