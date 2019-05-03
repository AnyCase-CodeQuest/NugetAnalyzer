using NugetAnalyzer.BLL.Entities;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    /// <summary>
    /// Service for comparing the current version of the package with the current and publication date of the package with the current date
    /// </summary>
    public interface IPackageVersionComparerService
    {
        /// <summary>
        /// Comparing the current version of the package with the current and publication date of the package with the current date
        /// </summary>
        /// <param name="actualVersion">Latest released package version</param>
        /// <param name="currentVersion">Current package version</param>
        /// <returns>Comparison result consisting of version status and date status</returns>
        PackageVersionComparisonResult Compare(PackageVersion actualVersion, PackageVersion currentVersion);
    }
}
