using System.Linq;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Solutions;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Converters
{
    public class SolutionConverter
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
                    Projects = solution.Projects.Select(ProjectConverter.ProjectToProjectWithVersionReport).ToList()
                };
        }
    }
}
