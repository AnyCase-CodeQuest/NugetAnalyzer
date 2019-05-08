using System;
using System.Collections.Generic;
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
        private Mock<IDateTimeProvider> dateTimeProviderMock;
        private IVersionService versionService;

        [OneTimeSetUp]
        public void Init()
        {
            uowMock = new Mock<IUnitOfWork>();
            versionRepositoryMock = new Mock<IVersionRepository>();
            dateTimeProviderMock = new Mock<IDateTimeProvider>();

            uowMock
                .Setup(p => p.GetRepository<IVersionRepository>())
                .Returns(versionRepositoryMock.Object);

            versionService = new VersionService(uowMock.Object, dateTimeProviderMock.Object);
        }

        [Test]
        public void UpdateLatestVersionOfNewPackagesAsync_Should_Invoke_Add_And_Update_When_Valid_Values()
        {
            versionRepositoryMock
                .Setup(p => p.GetLatestVersionsAsync(It.IsAny<Expression<Func<PackageVersion, bool>>>()))
                .ReturnsAsync(GetLatestPackageVersions);

            versionService.UpdateLatestVersionOfNewPackagesAsync(GetPackageVersions());

            versionRepositoryMock.Verify(x => x.Add(It.IsAny<PackageVersion>()));
            versionRepositoryMock.Verify(x => x.Update(It.IsAny<PackageVersion>()));
            uowMock.Verify(p => p.SaveChangesAsync());
        }

        [Test]
        public void UpdateLatestVersionOfPackagesAsync_Should_Invoke_Add_And_Update_When_Valid_Values()
        {
            versionRepositoryMock
                .Setup(p => p.GetAllLatestVersionsAsync())
                .ReturnsAsync(GetLatestPackageVersions);

            versionService.UpdateLatestVersionOfPackagesAsync(GetPackageVersions());

            versionRepositoryMock.Verify(x => x.Add(It.IsAny<PackageVersion>()));
            versionRepositoryMock.Verify(x => x.Update(It.IsAny<PackageVersion>()));
            uowMock.Verify(p => p.SaveChangesAsync());
        }

        private IReadOnlyCollection<PackageVersion> GetPackageVersions()
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

        private IReadOnlyCollection<PackageVersion> GetLatestPackageVersions()
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