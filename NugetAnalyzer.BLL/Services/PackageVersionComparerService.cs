using NugetAnalyzer.BLL.Entities;
using NugetAnalyzer.BLL.Entities.Enums;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain;
using System;
using Microsoft.Extensions.Options;

namespace NugetAnalyzer.BLL.Services
{
    public class PackageVersionComparerService : IPackageVersionComparerService
    {
        private readonly PackageVersionConfiguration packageVersionConfiguration;
        public PackageVersionComparerService(IOptions<PackageVersionConfiguration> packageVersionConfiguration)
        {
            this.packageVersionConfiguration = packageVersionConfiguration.Value ?? throw new ArgumentNullException(nameof(packageVersionConfiguration));
        }
        public PackageVersionComparisonResult Compare(PackageVersion latestVersion, PackageVersion currentVersion)
        {
            if(latestVersion == null)
                throw new ArgumentNullException(nameof(latestVersion));
            if (currentVersion == null)
                throw new ArgumentNullException(nameof(currentVersion));

            var comparisonResult = new PackageVersionComparisonResult
            {
                VersionStatus = CompareVersions(latestVersion, currentVersion),
                DateStatus = CompareDates(latestVersion.PublishedDate, currentVersion.PublishedDate),
                IsObsolete = ObsoleteCheck(latestVersion.PublishedDate)
            };

            return comparisonResult;
        }

        private bool ObsoleteCheck(DateTime? publishedDateOfLatestVersion)
        {
            if (publishedDateOfLatestVersion == null)
                return false;
            return DateTime.Today.Subtract(publishedDateOfLatestVersion.Value).Days / (365.25 / 12) >=
                   packageVersionConfiguration.ObsoleteBorderInMonths; // TODO: DateTime.Today to TimeProvider
        }

        private PackageDateStatus CompareDates(DateTime? publishedDateOfLatestVersion, DateTime? publishedDateOfCurrentVersion)
        {
            if (publishedDateOfLatestVersion == null || publishedDateOfCurrentVersion == null)
                return PackageDateStatus.Undefined;

            var differenceInMonths =
                publishedDateOfLatestVersion.Value.Subtract(publishedDateOfCurrentVersion.Value).Days / (365.25 / 12);

            if (differenceInMonths < packageVersionConfiguration.DateBordersInMonths.WarningBottomBorder)
                return PackageDateStatus.Normal;
            if (differenceInMonths < packageVersionConfiguration.DateBordersInMonths.ErrorBottomBorder)
                return PackageDateStatus.Warning;
            return PackageDateStatus.Error;
        }

        private PackageVersionStatus CompareVersions(PackageVersion latestVersion, PackageVersion currentVersion)
        {
            if (!latestVersion.Major.Equals(currentVersion.Major))
                return packageVersionConfiguration.VersionStatus.Major;
            if (!latestVersion.Minor.Equals(currentVersion.Minor))
                return packageVersionConfiguration.VersionStatus.Minor;
            if (!latestVersion.Build.Equals(currentVersion.Build))
                return packageVersionConfiguration.VersionStatus.Build;
            if (!latestVersion.Revision.Equals(currentVersion.Revision))
                return packageVersionConfiguration.VersionStatus.Revision;
            return PackageVersionStatus.Actual;
        }
    }
}
