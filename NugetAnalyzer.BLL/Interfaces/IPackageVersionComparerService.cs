using NugetAnalyzer.BLL.Entities;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    /// <summary>
    /// Service for comparing the current version of the package with the latest version and publication date of the current package with the publication date of latest package
    /// </summary>
    public interface IPackageVersionComparerService
    {
        /// <summary>
        /// Comparing the current version of the package with the latest version and publication date of the current package with the publication date of latest package
        /// </summary>
        /// <param name="latestVersion">Latest released package version</param>
        /// <param name="currentVersion">Current package version</param>
        /// <returns>Comparison result which consisting of version and date statuses of current package and relevance of latest package version</returns>
        PackageVersionComparisonResult Compare(PackageVersion latestVersion, PackageVersion currentVersion);
    }
}
