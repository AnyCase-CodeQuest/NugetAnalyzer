using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SolutionId { get; set; }

        public Solution Solution { get; set; }

        public ICollection<ProjectPackageVersion> ProjectPackageVersions { get; set; }
    }
}