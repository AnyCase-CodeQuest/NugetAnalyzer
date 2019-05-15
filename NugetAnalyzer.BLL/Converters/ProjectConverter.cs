using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Projects;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Converters
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
