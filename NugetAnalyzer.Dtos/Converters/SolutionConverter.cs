using System;
using System.Linq;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models;
using NugetAnalyzer.Dtos.Models.Solutions;

namespace NugetAnalyzer.Dtos.Converters
{
    public static class SolutionConverter
    {
        public static SolutionWithVersionReport SolutionToSolutionWithVersionReport(Solution solution)
        {
            return solution == null
                ? null
                : new SolutionWithVersionReport
                {
                    Id = solution.Id,
                    Name = solution.Name,
                    Report = new PackageVersionComparisonReport(),
                    Projects = solution.Projects == null 
                        ? throw new ArgumentNullException(nameof(solution.Projects))
                        : solution.Projects
                            .Select(ProjectConverter.ProjectToProjectWithVersionReport)
                            .ToList()
                };
        }
    }
}
