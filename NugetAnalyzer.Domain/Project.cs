using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }

        public int SolutionId { get; set; }

        public Solution Solution { get; set; }

        public ICollection<ProjectPackageVersion> ProjectPackageVersions { get; set; } =
            new List<ProjectPackageVersion>();
    }
}