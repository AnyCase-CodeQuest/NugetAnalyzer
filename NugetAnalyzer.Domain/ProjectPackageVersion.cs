namespace NugetAnalyzer.Domain
{
    public class ProjectPackageVersion
    {
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public int PackageVersionId { get; set; }

        public PackageVersion PackageVersion { get; set; }
    }
}