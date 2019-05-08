using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Models.Projects
{
    public class ProjectWithVersionReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PackageVersionComparisonReport Report { get; set; }
    }
}
