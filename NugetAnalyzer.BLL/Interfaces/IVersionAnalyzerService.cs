using System.Collections.Generic;
using NugetAnalyzer.Dtos.Models;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    /// <summary>
    /// Service for comparing versions of packages and calculating the maximum in importance report of reports
    /// </summary>
    public interface IVersionAnalyzerService
    {
        /// <summary>
        /// Comparing the current version of the package with the latest version and publication date of the current package with the publication date of latest package
        /// </summary>
        /// <param name="latestVersion">Latest released package version</param>
        /// <param name="currentVersion">Current package version</param>
        /// <returns>Report which consisting of version and date statuses of current package and relevance of latest package version</returns>
        PackageVersionComparisonReport Compare(PackageVersion latestVersion, PackageVersion currentVersion);

        /// <summary>
        /// Calculates the maximum level of the status of the date, version and obsolete status
        /// </summary>
        /// <param name="reports">list of reports from which to calculate</param>
        /// <returns>Report which consisting maximum level of the status of the date, version and obsolete status</returns>
        PackageVersionComparisonReport CalculateMaxReportLevelStatus(ICollection<PackageVersionComparisonReport> reports);
    }
}
