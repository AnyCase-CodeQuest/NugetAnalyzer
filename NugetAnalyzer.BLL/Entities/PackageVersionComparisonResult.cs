using NugetAnalyzer.BLL.Entities.Enums;

namespace NugetAnalyzer.BLL.Entities
{
    public struct PackageVersionComparisonResult
    {
        public VersionStatus VersionStatus { get; set; }
        public PublicationDateStatus PublicationDateStatus { get; set; }
    }
}
