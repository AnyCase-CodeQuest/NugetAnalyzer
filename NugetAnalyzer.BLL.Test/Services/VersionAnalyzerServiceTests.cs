using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common.Configurations;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Enums;
using NugetAnalyzer.DTOs.Models.Reports;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class VersionAnalyzerServiceTests
    {
        private VersionsAnalyzerService versionsService;
        private Mock<IDateTimeProvider> dateTimeProviderMock;

        private readonly IOptions<PackageVersionConfigurations> packageVersionConfiguration = Options.Create(new PackageVersionConfigurations
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
        });

        private readonly PackageVersion latestPackageVersion = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = DateTime.Today
        };
        private readonly PackageVersion obsoletePackageVersion = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = DateTime.Today.AddYears(-2)
        };

        private readonly PackageVersion actualPackageVersion = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = DateTime.Today.AddMonths(-1)
        };

        private readonly PackageVersion packageVersionWithErrorVersionStatus = new PackageVersion
        {
            Major = 0,
            Minor = 1,
            Build = 1,
            Revision = 0,
            PublishedDate = DateTime.Today.AddMonths(-1)
        };

        private readonly PackageVersion packageVersionWithErrorDateStatus = new PackageVersion
        {
            Major = 1,
            Minor = 1,
            Build = 1,
            Revision = 1,
            PublishedDate = DateTime.Today.AddYears(-3).AddMonths(-1)
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
            dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.SetupGet(dateTimeProvider => dateTimeProvider.CurrentUtcDateTime).Returns(DateTime.UtcNow);

            versionsService = new VersionsAnalyzerService(packageVersionConfiguration, dateTimeProviderMock.Object);
        }

        [Test] public void Constructor_Should_ThrowsArgumentNullException_When_AnyArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new VersionsAnalyzerService(null, dateTimeProviderMock.Object));
            Assert.Throws<ArgumentNullException>(() => new VersionsAnalyzerService(packageVersionConfiguration,null));
        }

        [Test]
        public void Compare_Should_ThrowsArgumentNullException_When_LatestVersionArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => versionsService.Compare(null, actualPackageVersion));
        }

        [Test]
        public void Compare_Should_ThrowsArgumentNullException_When_CurrentVersionArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => versionsService.Compare(latestPackageVersion, null));
        }

        [Test]
        public void Compare_Should_ReturnsReportWithObsoleteTrueValue_When_LatestVersionIsObsolete()
        {
            PackageVersionComparisonReport report = versionsService.Compare(obsoletePackageVersion, packageVersionWithErrorDateStatus);

            Assert.AreEqual(report.IsObsolete, true);
        }

        [Test]
        public void Compare_Should_ReturnsReportWithUndefinedDateValueAndFalseObsoleteValue_When_DateOfVersionIsUndefined()
        {
            PackageVersionComparisonReport report = versionsService.Compare(packageVersionWithUndefinedDate, packageVersionWithUndefinedDate);

            Assert.AreEqual(report.DateStatus, PackageVersionStatus.Undefined);
            Assert.AreEqual(report.IsObsolete, false);
        }

        [Test]
        public void Compare_Should_ReturnsReportWithDateErrorValue_When_DateOfCurrentVersionIsOld()
        {
            PackageVersionComparisonReport report = versionsService.Compare(latestPackageVersion, packageVersionWithErrorDateStatus);

            Assert.AreEqual(report.DateStatus, PackageVersionStatus.Error);
            Assert.AreEqual(report.VersionStatus, PackageVersionStatus.Actual);
        }

        [Test]
        public void Compare_Should_ReturnsReportWithVersionErrorValue_When_MajorVersionOfCurrentVersionIsChanged()
        {
            PackageVersionComparisonReport report = versionsService.Compare(latestPackageVersion, packageVersionWithErrorVersionStatus);

            Assert.AreEqual(report.DateStatus, PackageVersionStatus.Actual);
            Assert.AreEqual(report.VersionStatus, PackageVersionStatus.Error);
        }

        [Test]
        public void CalculateMaxReportLevelStatus_Should_ThrowsArgumentNullException_When_ReportsArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => versionsService.CalculateMaxReportLevelStatus(null));
        }

        [Test]
        public void CalculateMaxReportLevelStatus_Should_ReturnsReportWithAllGoodValues_When_ReportsCountIsZero()
        {
            PackageVersionComparisonReport report = versionsService.CalculateMaxReportLevelStatus(new List<PackageVersionComparisonReport>(0));

            Assert.AreEqual(report, new PackageVersionComparisonReport());
        }

        [Test]
        public void CalculateMaxReportLevelStatus_CheckReturnsValues()
        {
            PackageVersionComparisonReport report1 = versionsService.CalculateMaxReportLevelStatus(GetTestReports1());
            PackageVersionComparisonReport report2 = versionsService.CalculateMaxReportLevelStatus(GetTestReports2());

            Assert.AreEqual(report1, new PackageVersionComparisonReport
            {
                IsObsolete = true,
                VersionStatus = PackageVersionStatus.Error,
                DateStatus = PackageVersionStatus.Warning
            });
            Assert.AreEqual(report2, new PackageVersionComparisonReport
            {
                IsObsolete = true,
                VersionStatus = PackageVersionStatus.Info,
                DateStatus = PackageVersionStatus.Undefined
            });
        }

        private List<PackageVersionComparisonReport> GetTestReports1()
        {
            return new List<PackageVersionComparisonReport>
            {
                new PackageVersionComparisonReport
                {
                    VersionStatus = PackageVersionStatus.Actual
                },
                new PackageVersionComparisonReport
                {
                    IsObsolete = true,
                    VersionStatus = PackageVersionStatus.Warning,
                    DateStatus = PackageVersionStatus.Warning
                },
                new PackageVersionComparisonReport
                {
                    IsObsolete = false,
                    VersionStatus = PackageVersionStatus.Error,
                    DateStatus = PackageVersionStatus.Actual
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
                    DateStatus = PackageVersionStatus.Undefined
                },
                new PackageVersionComparisonReport
                {
                    IsObsolete = true,
                    VersionStatus = PackageVersionStatus.Info,
                    DateStatus = PackageVersionStatus.Undefined
                },
                new PackageVersionComparisonReport
                {
                    VersionStatus = PackageVersionStatus.Actual
                }
            };
        }
    }
}