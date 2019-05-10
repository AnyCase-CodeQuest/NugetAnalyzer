using System.Linq;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Converters
{
    internal class RepositoryConverter
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
                    Solutions = repository.Solutions.Select(SolutionConverter.SolutionToSolutionWithVersionReport).ToList()
                };
        }
    }
}
