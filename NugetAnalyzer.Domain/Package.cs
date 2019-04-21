namespace NugetAnalyzer.Domain
{
    public class Package : PackageBase
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? ReferencePackageId { get; set; }
        public ReferencePackage ReferencePackage { get; set; }
    }
}