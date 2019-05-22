using System;

namespace NugetAnalyzer.DTOs.Models.Reports
{
    public class PackageReport : BaseVersionReport
    {
        public DateTime? LastUpdateTime { get; set; } 

        public PackageVersionDTO CurrentPackageVersion { get; set; }

        public PackageVersionDTO LatestPackageVersion { get; set; }
    }
}