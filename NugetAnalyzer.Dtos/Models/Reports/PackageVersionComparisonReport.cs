using NugetAnalyzer.DTOs.Models.Enums;

namespace NugetAnalyzer.DTOs.Models.Reports
{
    public struct PackageVersionComparisonReport
    {
        public PackageVersionStatus VersionStatus { get; set; }

        public PackageVersionStatus DateStatus { get; set; }

        public bool IsObsolete { get; set; }
    }
}
