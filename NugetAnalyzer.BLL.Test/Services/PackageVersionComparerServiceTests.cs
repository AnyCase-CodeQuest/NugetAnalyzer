using System;
using Microsoft.Extensions.Options;
using NugetAnalyzer.BLL.Entities;
using NugetAnalyzer.BLL.Entities.Enums;
using NugetAnalyzer.BLL.Services;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class PackageVersionComparerServiceTests
    {
        private PackageVersionComparerService comparerService;

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

        [OneTimeSetUp]
        public void Init()
        {
            comparerService = new PackageVersionComparerService(Options.Create(packageVersionConfiguration));
        }

        [Test]
        public void Constructor_When_PackageVersionConfigurationArgumentIsNull_Should_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PackageVersionComparerService(null));
        }
    }
}