using System;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.DTOs.Models
{
    public class PackageReportDTO
    {
        public int PackageId { get; set; }

        public DateTime? LastUpdateTime { get; set; } 

        public string PackageName { get; set; }

        public PackageVersionDTO Current { get; set; }

        public PackageVersionDTO Latest { get; set; }

        public PackageVersionComparisonReport Report { get; set; }
    }
}