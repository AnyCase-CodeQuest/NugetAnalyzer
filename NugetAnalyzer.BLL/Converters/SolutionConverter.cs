using System.Collections.Generic;
using System.Linq;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Projects;
using NugetAnalyzer.BLL.Models.Solutions;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Converters
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
                        ? new List<ProjectWithVersionReport>()
                        : solution
                            .Projects
                            .Select(ProjectConverter.ProjectToProjectWithVersionReport)
                            .ToList()
                };
        }
    }
}
