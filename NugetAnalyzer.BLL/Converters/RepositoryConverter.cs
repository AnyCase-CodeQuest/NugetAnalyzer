using System.Collections.Generic;
using System.Linq;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.BLL.Models.Solutions;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Converters
{
    public static class RepositoryConverter
    {
        public static RepositoryWithVersionReport RepositoryToRepositoryWithVersionReport(Repository repository)
        {
            return repository == null
                ? null
                : new RepositoryWithVersionReport
                {
                    Id = repository.Id,
                    Name = repository.Name,
                    Report = new PackageVersionComparisonReport(),
                    Solutions = repository.Solutions == null
                        ? new List<SolutionWithVersionReport>()
                        : repository
                            .Solutions
                            .Select(SolutionConverter.SolutionToSolutionWithVersionReport)
                            .ToList()
                };
        }
    }
}
