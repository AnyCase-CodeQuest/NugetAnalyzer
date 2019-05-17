using NugetAnalyzer.Dtos.Models.Enums;

namespace NugetAnalyzer.Dtos.Models.Reports
{
    public struct PackageVersionComparisonReport
    {
        public PackageVersionStatus VersionStatus { get; set; }

        public PackageDateStatus DateStatus { get; set; }

        public bool IsObsolete { get; set; }
    }
}
