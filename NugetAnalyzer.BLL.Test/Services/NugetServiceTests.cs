using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NugetAnalyzer.BLL.Helpers;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class NugetServiceTests
    {
        private Mock<IPackageService> packageServiceMock;
        private Mock<INugetApiService> nugetApiServiceMock;
        private Mock<IPackageVersionService> versionServiceMock;
        private INugetService nugetService;

        private readonly PackageVersion nUnitVersion;
        private readonly PackageVersion moqVersion;
        private readonly Package nUnitPackage;
        private readonly Package moqPackage;
        private readonly List<PackageVersion> packageVersions;
        private readonly List<Package> packages;

        public NugetServiceTests()
        {
            nUnitVersion = new PackageVersion
            {
                Id = 1,
                Major = 1,
                Minor = 1,
                Build = -1,
                Revision = -1,
                PackageId = 1
            };

            moqVersion = new PackageVersion
            {
                Id = 2,
                Major = 2,
                Minor = 2,
                Build = 2,
                Revision = -1,
                PackageId = 2
            };

            nUnitPackage = new Package
            {
                Id = 1,
                Name = "NUnit",
                Versions = new List<PackageVersion>
                {
                    nUnitVersion
                },
                LastUpdateTime = null
            };

            moqPackage = new Package
            {
                Id = 2,
                Name = "Moq",
                Versions = new List<PackageVersion>
                {
                    moqVersion
                },
                LastUpdateTime = null
            };

            packageVersions = new List<PackageVersion>
            {
                nUnitVersion,
                moqVersion
            };

            packages = new List<Package>
            {
                nUnitPackage,
                moqPackage
            };
        }

        [OneTimeSetUp]
        public void Init()
        {
            packageServiceMock = new Mock<IPackageService>();
            nugetApiServiceMock = new Mock<INugetApiService>();
            versionServiceMock = new Mock<IPackageVersionService>();

            nugetService = new NugetService(
                nugetApiServiceMock.Object,
                versionServiceMock.Object,
                packageServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            packageServiceMock
                .Setup(packageService => packageService.GetAllAsync())
                .ReturnsAsync(packages);

            packageServiceMock
                .Setup(packageService => packageService.GetNewlyAddedPackagesAsync())
                .ReturnsAsync(packages);

            nugetApiServiceMock
                .Setup(nugetApiService => nugetApiService.GetLatestPackageVersionAsync(nUnitPackage.Name))
                .ReturnsAsync(nUnitVersion);

            nugetApiServiceMock
                .Setup(nugetApiService => nugetApiService.GetLatestPackageVersionAsync(moqPackage.Name))
                .ReturnsAsync(moqVersion);

            nugetApiServiceMock
                .Setup(nugetApiService => nugetApiService.GetPackagePublishedDateByVersionAsync(
                    nUnitPackage.Name,
                    nUnitVersion
                        .GetVersion()
                        .ToString()))
                .ReturnsAsync(nUnitVersion.PublishedDate);

            nugetApiServiceMock
                .Setup(nugetApiService => nugetApiService.GetPackagePublishedDateByVersionAsync(
                    moqPackage.Name,
                    moqVersion
                        .GetVersion()
                        .ToString()))
                .ReturnsAsync(moqVersion.PublishedDate);
        }

        [Test]
        public async Task RefreshLatestVersionOfAllPackagesAsync_Should_Invokes_UpdateLatestVersionOfPackagesAsync_When_Valid_Values()
        {
            await nugetService.RefreshLatestVersionOfAllPackagesAsync();

            packageServiceMock.Verify(packageService => packageService.GetAllAsync());
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetLatestPackageVersionAsync(nUnitPackage.Name));
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetLatestPackageVersionAsync(moqPackage.Name));
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetPackagePublishedDateByVersionAsync(nUnitPackage.Name, nUnitVersion.GetVersion().ToString()));
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetPackagePublishedDateByVersionAsync(moqPackage.Name, moqVersion.GetVersion().ToString()));
            versionServiceMock.Verify(versionService => versionService.UpdateAllLatestVersionsAsync(packageVersions));
        }

        [Test]
        public async Task RefreshLatestVersionOfNewPackagesAsync_Should_Invokes_UpdateLatestVersionOfNewPackagesAsync_When_Valid_Values()
        {
            await nugetService.RefreshLatestVersionOfNewlyAddedPackagesAsync();

            packageServiceMock.Verify(packageService => packageService.GetNewlyAddedPackagesAsync());
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetLatestPackageVersionAsync(nUnitPackage.Name));
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetLatestPackageVersionAsync(moqPackage.Name));
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetPackagePublishedDateByVersionAsync(nUnitPackage.Name, nUnitVersion.GetVersion().ToString()));
            nugetApiServiceMock.Verify(nugetApiService => nugetApiService.GetPackagePublishedDateByVersionAsync(moqPackage.Name, moqVersion.GetVersion().ToString()));
            versionServiceMock.Verify(versionService => versionService.UpdateLatestVersionsAsync(packageVersions));
        }

        [Test]
        public async Task RefreshLatestVersionOfAllPackagesAsync_Should_Exclude_PackageVersions_When_Request_To_NugetApi_Throw_Exception()
        {
            nugetApiServiceMock
                .Setup(nugetApiService => nugetApiService.GetLatestPackageVersionAsync(moqPackage.Name))
                .ThrowsAsync(new Exception("Nuget API exception."));

            await nugetService.RefreshLatestVersionOfAllPackagesAsync();

            List<PackageVersion> versions = new List<PackageVersion>
            {
                nUnitVersion
            };
            versionServiceMock.Verify(versionService => versionService.UpdateAllLatestVersionsAsync(versions));
        }
    }
}