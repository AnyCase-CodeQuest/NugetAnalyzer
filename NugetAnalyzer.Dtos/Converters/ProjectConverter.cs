using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models;
using NugetAnalyzer.Dtos.Models.Projects;

namespace NugetAnalyzer.Dtos.Converters
{
    public static class ProjectConverter
    {
        public static ProjectWithVersionReport ProjectToProjectWithVersionReport(Project project)
        {
            return project == null
                ? null
                : new ProjectWithVersionReport
                {
                    Id = project.Id,
                    Name = project.Name,
                    Report = new PackageVersionComparisonReport()
                };
        }
    }
}
