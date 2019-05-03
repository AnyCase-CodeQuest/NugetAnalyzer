using NugetAnalyzer.BLL.Entities;
using NugetAnalyzer.BLL.Entities.Constants;
using NugetAnalyzer.BLL.Entities.Enums;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain;
using System;

namespace NugetAnalyzer.BLL.Services
{
    public class PackageVersionComparerService : IPackageVersionComparerService
    {
        public PackageVersionComparisonResult Compare(PackageVersion actualVersion, PackageVersion currentVersion)
        {
            if(actualVersion == null)
                throw new ArgumentNullException(nameof(actualVersion));
            if (currentVersion == null)
                throw new ArgumentNullException(nameof(currentVersion));

            var comparisonResult = new PackageVersionComparisonResult
            {
                VersionStatus = CompareVersion(actualVersion, currentVersion),
                PublicationDateStatus = CheckPublicationDate(actualVersion.PublishedDate)
            };

            return comparisonResult;
        }

        private PackagePublicationDateStatus CheckPublicationDate(DateTime? publishedDate)
        {
            if (publishedDate == null)
                return PackagePublicationDateStatus.Undefined;

            var difference = DateTime.Today.Subtract(publishedDate.Value).Days / (365.25 / 12); // TODO: DateTime.Today to TimeProvider

            if (difference < PackagePublicationDateConstants.NormalInMonthsTopBorder)
                return PackagePublicationDateStatus.Normal;
            if (difference < PackagePublicationDateConstants.ObsoleteInMonthsBottomBorder)
                return PackagePublicationDateStatus.HalfYearOld;
            return PackagePublicationDateStatus.Obsolete;
        }

        private PackageVersionStatus CompareVersion(PackageVersion actualVersion, PackageVersion currentVersion)
        {
            if (!actualVersion.Major.Equals(currentVersion.Major))
                return PackageVersionStatus.MajorChanged;
            if (!actualVersion.Minor.Equals(currentVersion.Minor))
                return PackageVersionStatus.MinorChanged;
            if (!actualVersion.Build.Equals(currentVersion.Build))
                return PackageVersionStatus.BuildChanged;
            if (!actualVersion.Revision.Equals(currentVersion.Revision))
                return PackageVersionStatus.RevisionChanged;
            return PackageVersionStatus.Actual;
        }
    }
}
