using System;
using System.Linq;
using NugetAnalyzer.Dtos.Models;
using NugetAnalyzer.Dtos.Models.Repositories;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.Dtos.Converters
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
                        ? throw new ArgumentNullException(nameof(repository.Solutions))
                        : repository.Solutions
                            .Select(SolutionConverter.SolutionToSolutionWithVersionReport)
                            .ToList()
                };
        }
    }
}
