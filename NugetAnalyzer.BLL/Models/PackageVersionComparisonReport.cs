using NugetAnalyzer.BLL.Models.Enums;

namespace NugetAnalyzer.BLL.Models
{
    public struct PackageVersionComparisonReport
    {
        public PackageVersionStatus VersionStatus { get; set; }
        public PackageDateStatus DateStatus { get; set; }
        public bool IsObsolete { get; set; }
    }
}
