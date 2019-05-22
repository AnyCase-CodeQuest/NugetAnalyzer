using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models.Reports
{
    public class ProjectReport : BaseVersionReport
    {
        /// <summary>
        /// Gets or sets the packages with reports
        /// Can be null to show no details information
        /// </summary>
        public List<PackageReport> Packages { get; set; }
    }
}