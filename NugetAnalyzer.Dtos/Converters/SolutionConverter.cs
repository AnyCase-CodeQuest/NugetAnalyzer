using System;
using System.Linq;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.DTOs.Converters
{
    public static class SolutionConverter
    {
        public static SolutionReport SolutionToSolutionVersionReport(Solution solution)
        {
            return solution == null
                ? null
                : new SolutionReport
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
