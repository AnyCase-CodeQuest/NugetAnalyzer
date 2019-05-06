using NugetAnalyzer.BLL.Entities.Enums;

namespace NugetAnalyzer.BLL.Entities
{
    public struct PackageVersionComparisonResult
    {
        public PackageVersionStatus VersionStatus { get; set; }
        public PackageDateStatus DateStatus { get; set; }
        public bool IsObsolete { get; set; }
    }
}
