using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Configurations;
using NugetAnalyzer.BLL.Models.Enums;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class VersionServiceTests
    {
        private VersionService versionService;
        private Mock<IDateTimeProvider> dateTimeProvider;
        private readonly PackageVersionConfiguration packageVersionConfiguration = new PackageVersionConfiguration
        {
            VersionStatus = new VersionConfigs
            {
                Major = PackageVersionStatus.Error,
                Minor = PackageVersionStatus.Warning,
                Build = PackageVersionStatus.Info,
                Revision = PackageVersionStatus.Info
            },
            DateBordersInMonths = new DateBordersInMonthsConfigs
            {
                WarningBottomBorder = 6,
                ErrorBottomBorder = 12
            },
            ObsoleteBorderInMonths = 12
        };

        private readonly PackageVersion latestPackageVersion = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = new DateTime(2019, 05, 10)
        };
        private readonly PackageVersion obsoletePackageVersion = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = new DateTime(2017, 05, 10)
        };

        private readonly PackageVersion actualPackageVersion = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = new DateTime(2019, 04, 4)
        };

        private readonly PackageVersion packageVersionWithErrorVersionStatus = new PackageVersion
        {
            Major = 0,
            Minor = 1,
            Build = 1,
            Revision = 0,
            PublishedDate = new DateTime(2019, 04, 4)
        };

        private readonly PackageVersion packageVersionWithErrorDateStatus = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = new DateTime(2016, 04, 4)
        };

        private readonly PackageVersion packageVersionWithUndefinedDate = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = null
        };

        [OneTimeSetUp]
        public void Init()
        {
            dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.SetupGet(p => p.CurrentUtcDateTime).Returns(DateTime.UtcNow);

            versionService = new VersionService(Options.Create(packageVersionConfiguration), dateTimeProvider.Object);
        }

        [Test]
        public void Compare_When_LatestVersionArgumentIsNull_Should_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => versionService.Compare(null, actualPackageVersion));
        }

        [Test]
        public void Compare_When_CurrentVersionArgumentIsNull_Should_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => versionService.Compare(latestPackageVersion, null));
        }

        [Test]
        public void Compare_When_LatestVersionIsObsolete_Should_ReturnsReportWithObsoleteTrueValue()
        {
            var report = versionService.Compare(obsoletePackageVersion, packageVersionWithErrorDateStatus);

            Assert.AreEqual(report.IsObsolete, true);
        }

        [Test]
        public void Compare_When_DateOfVersionIsUndefined_Should_ReturnsReportWithUndefinedDateValueAndFalseObsoleteValue()
        {
            var report = versionService.Compare(packageVersionWithUndefinedDate, packageVersionWithUndefinedDate);

            Assert.AreEqual(report.DateStatus, PackageDateStatus.Undefined);
            Assert.AreEqual(report.IsObsolete, false);
        }

        [Test]
        public void Compare_When_DateOfCurrentVersionIsOld_Should_ReturnsReportWithDateErrorValue()
        {
            var report = versionService.Compare(latestPackageVersion, packageVersionWithErrorDateStatus);

            Assert.AreEqual(report.DateStatus, PackageDateStatus.Error);
            Assert.AreEqual(report.VersionStatus, PackageVersionStatus.Actual);
        }

        [Test]
        public void Compare_When_MajorVersionOfCurrentVersionIsChanged_Should_ReturnsReportWithVersionErrorValue()
        {
            var report = versionService.Compare(latestPackageVersion, packageVersionWithErrorVersionStatus);

            Assert.AreEqual(report.DateStatus, PackageDateStatus.Normal);
            Assert.AreEqual(report.VersionStatus, PackageVersionStatus.Error);
        }

        [Test]
        public void CalculateMaxReportLevelStatus_When_ReportsArgumentIsNull_Should_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => versionService.CalculateMaxReportLevelStatus(null));
        }

        [Test]
        public void CalculateMaxReportLevelStatus_When_ReportsCountIsZero_Should_ReturnsReportWithAllGoodValues()
        {
            var report = versionService.CalculateMaxReportLevelStatus(new List<PackageVersionComparisonReport>(0));

            Assert.AreEqual(report, new PackageVersionComparisonReport());
        }

        [Test]
        public void CalculateMaxReportLevelStatus_CheckReturnsValues()
        {
            var report1 = versionService.CalculateMaxReportLevelStatus(GetTestReports1());
            var report2 = versionService.CalculateMaxReportLevelStatus(GetTestReports2());

            Assert.AreEqual(report1, new PackageVersionComparisonReport
            {
                IsObsolete = true,
                VersionStatus = PackageVersionStatus.Error,
                DateStatus = PackageDateStatus.Warning
            });
            Assert.AreEqual(report2, new PackageVersionComparisonReport
            {
                IsObsolete = true,
                VersionStatus = PackageVersionStatus.Info,
                DateStatus = PackageDateStatus.Undefined
            });
        }

        private List<PackageVersionComparisonReport> GetTestReports1()
        {
            return new List<PackageVersionComparisonReport>
            {
                new PackageVersionComparisonReport(),
                new PackageVersionComparisonReport
                {
                    IsObsolete = true,
                    VersionStatus = PackageVersionStatus.Warning,
                    DateStatus = PackageDateStatus.Warning
                },
                new PackageVersionComparisonReport
                {
                    IsObsolete = false,
                    VersionStatus = PackageVersionStatus.Error,
                    DateStatus = PackageDateStatus.Normal
                }
            };
        }

        private List<PackageVersionComparisonReport> GetTestReports2()
        {
            return new List<PackageVersionComparisonReport>
            {
                new PackageVersionComparisonReport
                {
                    IsObsolete = false,
                    VersionStatus = PackageVersionStatus.Actual,
                    DateStatus = PackageDateStatus.Undefined
                },
                new PackageVersionComparisonReport
                {
                    IsObsolete = true,
                    VersionStatus = PackageVersionStatus.Info,
                    DateStatus = PackageDateStatus.Undefined
                },
                new PackageVersionComparisonReport()
            };
        }
    }
}