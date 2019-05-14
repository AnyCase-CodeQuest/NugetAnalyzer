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
        private Mock<IVersionRepository> versionRepositoryMock;
        private Mock<IRepository<Package>> packageRepositoryMock;
        private Mock<IDateTimeProvider> dateTimeProviderMock;
        private IVersionService versionService;

        [OneTimeSetUp]
        public void Init()
        {
            uowMock = new Mock<IUnitOfWork>();
            versionRepositoryMock = new Mock<IVersionRepository>();
            packageRepositoryMock = new Mock<IRepository<Package>>();
            dateTimeProviderMock = new Mock<IDateTimeProvider>();

            uowMock
                .Setup(uow => uow.VersionRepository)
                .Returns(versionRepositoryMock.Object);

            uowMock
                .Setup(uow => uow.GetRepository<Package>())
                .Returns(packageRepositoryMock.Object);

            versionService = new VersionService(uowMock.Object, dateTimeProviderMock.Object);
        }

        [Test]
        public void UpdateLatestVersionOfNewPackagesAsync_Should_Invoke_Add_And_Update_When_Valid_Values()
        {
            var latestVersions = GetLatestPackageVersions();
            var versions = GetPackageVersions();

            versionRepositoryMock
                .Setup(v => v.GetLatestVersionsAsync(It.IsAny<Expression<Func<PackageVersion, bool>>>()))
                .ReturnsAsync(latestVersions);

            versionService.UpdateLatestVersionsAsync(versions);

            latestVersions.ForEach(pv =>
                    packageRepositoryMock.Verify(repo => repo.Update(pv.Package)));

            versionRepositoryMock.Verify(repo => 
                repo.Add(It.Is<PackageVersion>(pv => pv == versions.FirstOrDefault(v => v.Id == 2))));

            versionRepositoryMock.Verify(repo =>
                repo.Update(It.Is<PackageVersion>(pv => pv == latestVersions.FirstOrDefault(v => v.Id == 1))));

            uowMock.Verify(uow => uow.SaveChangesAsync());
        }

        [Test]
        public void UpdateLatestVersionOfPackagesAsync_Should_Invoke_Add_And_Update_When_Valid_Values()
        {
            var latestVersions = GetLatestPackageVersions();
            var versions = GetPackageVersions();

            versionRepositoryMock
                .Setup(v => v.GetAllLatestVersionsAsync())
                .ReturnsAsync(latestVersions);

            versionService.UpdateAllLatestVersionsAsync(versions);

            latestVersions.ForEach(pv =>
                packageRepositoryMock.Verify(repo => repo.Update(pv.Package)));

            versionRepositoryMock.Verify(repo =>
                repo.Add(It.Is<PackageVersion>(pv => pv == versions.FirstOrDefault(v => v.Id == 2))));

            versionRepositoryMock.Verify(repo =>
                repo.Update(It.Is<PackageVersion>(pv => pv == latestVersions.FirstOrDefault(v => v.Id == 1))));

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