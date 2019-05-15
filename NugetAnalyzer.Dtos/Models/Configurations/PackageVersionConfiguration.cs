using NugetAnalyzer.Dtos.Models.Enums;

namespace NugetAnalyzer.Dtos.Models.Configurations
{
    public class PackageVersionConfiguration
    {
        public VersionConfigs VersionStatus { get; set; }
        public DateBordersInMonthsConfigs DateBordersInMonths { get; set; }
        public int ObsoleteBorderInMonths { get; set; }
    }

    public class DateBordersInMonthsConfigs
    {
        public int WarningBottomBorder { get; set; }
        public int ErrorBottomBorder { get; set; }
    }

    public class VersionConfigs
    {
        public PackageVersionStatus Major { get; set; }
        public PackageVersionStatus Minor { get; set; }
        public PackageVersionStatus Build { get; set; }
        public PackageVersionStatus Revision { get; set; }
    }
}
