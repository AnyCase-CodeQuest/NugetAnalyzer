using System.Collections.Generic;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.DTOs.Models
{
    public class ProjectReportDTO : ProjectVersionReport
    {
        public List<PackageReportDTO> Packages { get; set; }
    }
}