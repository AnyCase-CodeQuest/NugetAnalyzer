using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Enums;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using NugetAnalyzer.BLL.Models.Configurations;
using NugetAnalyzer.Common.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class VersionAnalyzerService : IVersionAnalyzerService
    {
        private const double daysInTheMonth = 365.25 / 12;

        private readonly PackageVersionConfiguration packageVersionConfiguration;
        private readonly IDateTimeProvider dateTimeProvider;

        public VersionAnalyzerService(IOptions<PackageVersionConfiguration> packageVersionConfiguration, IDateTimeProvider dateTimeProvider)
        {
            if (packageVersionConfiguration == null)
                throw new ArgumentNullException(nameof(packageVersionConfiguration));
            this.packageVersionConfiguration = packageVersionConfiguration.Value;
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public PackageVersionComparisonReport Compare(PackageVersion latestVersion, PackageVersion currentVersion)
        {
            if (latestVersion == null)
                throw new ArgumentNullException(nameof(latestVersion));
            if (currentVersion == null)
                throw new ArgumentNullException(nameof(currentVersion));

            return new PackageVersionComparisonReport
            {
                VersionStatus = CompareVersions(latestVersion, currentVersion),
                DateStatus = CompareDates(latestVersion.PublishedDate, currentVersion.PublishedDate),
                IsObsolete = ObsoleteCheck(latestVersion.PublishedDate)
            };
        }

        public PackageVersionComparisonReport CalculateMaxReportLevelStatus(ICollection<PackageVersionComparisonReport> reports)
        {
            if (reports == null)
                throw new ArgumentNullException(nameof(reports));

            return reports.Count == 0 ? new PackageVersionComparisonReport() : new PackageVersionComparisonReport
            {
                DateStatus = CalculateMaxDateStatusLevel(reports),
                VersionStatus = CalculateMaxVersionStatusLevel(reports),
                IsObsolete = reports.Cast<PackageVersionComparisonReport?>().FirstOrDefault(r => r.Value.IsObsolete) != null
            };
        }

        private PackageVersionStatus CalculateMaxVersionStatusLevel(ICollection<PackageVersionComparisonReport> reports)
        {
            return reports.Select(r => r.VersionStatus).Max();
        }

        private PackageDateStatus CalculateMaxDateStatusLevel(ICollection<PackageVersionComparisonReport> reports)
        {
            return reports.Select(r => r.DateStatus).Max();
        }

        private bool ObsoleteCheck(DateTime? publishedDateOfLatestVersion)
        {
            if (publishedDateOfLatestVersion == null)
                return false;
            return dateTimeProvider.CurrentUtcDateTime.Subtract(publishedDateOfLatestVersion.Value).Days / daysInTheMonth >=
                   packageVersionConfiguration.ObsoleteBorderInMonths;
        }

        private PackageDateStatus CompareDates(DateTime? publishedDateOfLatestVersion, DateTime? publishedDateOfCurrentVersion)
        {
            if (publishedDateOfLatestVersion == null || publishedDateOfCurrentVersion == null)
                return PackageDateStatus.Undefined;

            var differenceInMonths =
                publishedDateOfLatestVersion.Value.Subtract(publishedDateOfCurrentVersion.Value).Days / daysInTheMonth;

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
