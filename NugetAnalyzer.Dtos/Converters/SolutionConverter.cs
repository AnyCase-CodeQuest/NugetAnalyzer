using System;
using System.Linq;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;

namespace NugetAnalyzer.Dtos.Converters
{
    public static class SolutionConverter
    {
        public static SolutionVersionReport SolutionToSolutionVersionReport(Solution solution)
        {
            return solution == null
                ? null
                : new SolutionVersionReport
                {
                    Id = solution.Id,
                    Name = solution.Name,
                    Projects = solution.Projects == null 
                        ? throw new ArgumentNullException(nameof(solution.Projects))
                        : solution.Projects
                            .Select(ProjectConverter.ProjectToProjectVersionReport)
                            .ToList()
                };
        }
    }
}
