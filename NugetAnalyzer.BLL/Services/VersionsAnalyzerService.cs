using NugetAnalyzer.DTOs.Models.Enums;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using NugetAnalyzer.Common.Configurations;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.BLL.Services
{
    public class VersionsAnalyzerService : IVersionsAnalyzerService
    {
        private const double DaysInTheMonth = 365.25 / 12;

        private readonly PackageVersionConfigurations packageVersionConfiguration;
        private readonly IDateTimeProvider dateTimeProvider;

        public VersionsAnalyzerService(IOptions<PackageVersionConfigurations> packageVersionConfiguration, IDateTimeProvider dateTimeProvider)
        {
            if (packageVersionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(packageVersionConfiguration));
            }
            this.packageVersionConfiguration = packageVersionConfiguration.Value;
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public PackageVersionComparisonReport Compare(PackageVersion latestVersion, PackageVersion currentVersion)
        {
            if (latestVersion == null)
            {
                throw new ArgumentNullException(nameof(latestVersion));
            }
            if (currentVersion == null)
            {
                throw new ArgumentNullException(nameof(currentVersion));
            }

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
            {
                throw new ArgumentNullException(nameof(reports));
            }

            return reports.Count == 0 ? new PackageVersionComparisonReport() : new PackageVersionComparisonReport
            {
                VersionStatus = CalculateMaxVersionStatusLevel(reports),
                DateStatus = CalculateMaxDateStatusLevel(reports),
                IsObsolete = reports
                                 .Cast<PackageVersionComparisonReport?>()
                                 .FirstOrDefault(packageVersionComparisonReport => packageVersionComparisonReport.Value.IsObsolete) != null
            };
        }

        private PackageVersionStatus CalculateMaxVersionStatusLevel(ICollection<PackageVersionComparisonReport> reports)
        {
            return reports.Select(packageVersionComparisonReport => packageVersionComparisonReport.VersionStatus).Max();
        }

        private PackageVersionStatus CalculateMaxDateStatusLevel(ICollection<PackageVersionComparisonReport> reports)
        {
            return reports.Select(packageVersionComparisonReport => packageVersionComparisonReport.DateStatus).Max();
        }

        private bool ObsoleteCheck(DateTime? publishedDateOfLatestVersion)
        {
            if (publishedDateOfLatestVersion == null)
            {
                return false;
            }

            return dateTimeProvider
                       .CurrentUtcDateTime
                       .Subtract(publishedDateOfLatestVersion.Value)
                       .Days / DaysInTheMonth >= packageVersionConfiguration.ObsoleteBorderInMonths;
        }

        private PackageVersionStatus CompareDates(DateTime? publishedDateOfLatestVersion, DateTime? publishedDateOfCurrentVersion)
        {
            if (publishedDateOfLatestVersion == null || publishedDateOfCurrentVersion == null)
            {
                return PackageVersionStatus.Undefined;
            }

            double differenceInMonths = publishedDateOfLatestVersion.Value
                    .Subtract(publishedDateOfCurrentVersion.Value).Days / DaysInTheMonth;

            if (differenceInMonths < packageVersionConfiguration.DateBordersInMonths.WarningBottomBorder)
            {
                return PackageVersionStatus.Actual;
            }
            if (differenceInMonths < packageVersionConfiguration.DateBordersInMonths.ErrorBottomBorder)
            {
                return PackageVersionStatus.Warning;
            }

            return PackageVersionStatus.Error;
        }

        private PackageVersionStatus CompareVersions(PackageVersion latestVersion, PackageVersion currentVersion)
        {
            if (!latestVersion.Major.Equals(currentVersion.Major))
            {
                return packageVersionConfiguration.VersionStatus.Major;
            }
            if (!latestVersion.Minor.Equals(currentVersion.Minor))
            {
                return packageVersionConfiguration.VersionStatus.Minor;
            }
            if (!latestVersion.Build.Equals(currentVersion.Build))
            {
                return packageVersionConfiguration.VersionStatus.Build;
            }
            if (!latestVersion.Revision.Equals(currentVersion.Revision))
            {
                return packageVersionConfiguration.VersionStatus.Revision;
            }

            return PackageVersionStatus.Actual;
        }
    }
}
