using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class VersionServicesTests
    {
        private Mock<IUnitOfWork> uowMock;
        private Mock<IPackageVersionsRepository> packageVersionsRepositoryMock;
        private Mock<IPackagesRepository> packageRepositoryMock;
        private Mock<IDateTimeProvider> dateTimeProviderMock;
        private IPackageVersionService packageVersionService;

        [OneTimeSetUp]
        public void Init()
        {
            uowMock = new Mock<IUnitOfWork>();
            packageVersionsRepositoryMock = new Mock<IPackageVersionsRepository>();
            packageRepositoryMock = new Mock<IPackagesRepository>();
            dateTimeProviderMock = new Mock<IDateTimeProvider>();

            uowMock
                .Setup(uow => uow.PackageVersionsRepository)
                .Returns(packageVersionsRepositoryMock.Object);

            uowMock
                .Setup(uow => uow.GetRepository<Package>())
                .Returns(packageRepositoryMock.Object);

            packageVersionService = new PackageVersionService(uowMock.Object, dateTimeProviderMock.Object);
        }

        //TODO: fix next commit
        [Ignore("")]
        [Test]
        public void UpdateLatestVersionOfNewPackagesAsync_Should_Invoke_Add_And_Update_When_Valid_Values()
        {
            List<PackageVersion> latestVersions = GetLatestPackageVersions();
            List<PackageVersion> versions = GetPackageVersions();

            packageVersionsRepositoryMock
                .Setup(versionRepository => versionRepository.GetLatestVersionsAsync(It.IsAny<Expression<Func<PackageVersion, bool>>>()))
                .ReturnsAsync(latestVersions);

            packageVersionService.UpdateLatestVersionsAsync(versions);

            latestVersions.ForEach(packageVersion =>
                    packageRepositoryMock.Verify(packageRepository => packageRepository.Update(packageVersion.Package)));

            packageVersionsRepositoryMock.Verify(versionRepository =>
                versionRepository.Add(It.Is<PackageVersion>(packageVersion => packageVersion == versions.FirstOrDefault(version => version.Id == 2))));

            packageVersionsRepositoryMock.Verify(versionRepository =>
                versionRepository.Update(It.Is<PackageVersion>(packageVersion => packageVersion == latestVersions.FirstOrDefault(latestVersion => latestVersion.Id == 1))));

            uowMock.Verify(uow => uow.SaveChangesAsync());
        }

        //TODO: fix next commit
        [Ignore("")]
        [Test]
        public void UpdateLatestVersionOfPackagesAsync_Should_Invoke_Add_And_Update_When_Valid_Values()
        {
            List<PackageVersion> latestVersions = GetLatestPackageVersions();
            List<PackageVersion> versions = GetPackageVersions();

            packageVersionsRepositoryMock
                .Setup(versionRepository => versionRepository.GetAllLatestVersionsAsync())
                .ReturnsAsync(latestVersions);

            packageVersionService.UpdateAllLatestVersionsAsync(versions);

            latestVersions.ForEach(packageVersion =>
                packageRepositoryMock.Verify(packageRepository => packageRepository.Update(packageVersion.Package)));

            packageVersionsRepositoryMock.Verify(versionRepository =>
                versionRepository.Add(It.Is<PackageVersion>(packageVersion => packageVersion == versions.FirstOrDefault(version => version.Id == 2))));

            packageVersionsRepositoryMock.Verify(versionRepository =>
                versionRepository.Update(It.Is<PackageVersion>(packageVersion => packageVersion == latestVersions.FirstOrDefault(latestVersion => latestVersion.Id == 1))));

            uowMock.Verify(uow => uow.SaveChangesAsync());
        }

        private List<PackageVersion> GetPackageVersions()
        {
            return new List<PackageVersion>
            {
                new PackageVersion
                {
                    Id = 1,
                    Major = 1,
                    Minor = 0,
                    Build = 0,
                    Revision = 0,
                    PackageId = 1,
                    Package = new Package()
                },
                new PackageVersion
                {
                    Id = 2,
                    Major = 3,
                    Minor = 0,
                    Build = 0,
                    Revision = 0,
                    PackageId = 2,
                    Package = new Package()
                },
            };
        }

        private List<PackageVersion> GetLatestPackageVersions()
        {
            return new List<PackageVersion>
            {
                new PackageVersion
                {
                    Id = 1,
                    Major = 1,
                    Minor = 0,
                    Build = 0,
                    Revision = 0,
                    PackageId = 1,
                    Package = new Package()
                },
                new PackageVersion
                {
                    Id = 2,
                    Major = 2,
                    Minor = 0,
                    Build = 0,
                    Revision = 0,
                    PackageId = 2,
                    Package = new Package()
                },
            };
        }
    }
}