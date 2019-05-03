using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class NugetServiceTests
    {
        private Mock<IRepository<Package>> packageRepositoryMock;
        private Mock<INugetApiService> nugetApiServiceMock;
        private Mock<IVersionService> versionServiceMock;
        private Mock<IUnitOfWork> uowMock;
        private INugetService nugetService;
        private readonly PackageVersion testVersion = new PackageVersion
        {
            Id = 1,
            Major = 1,
            Minor = 1,
            Build = -1,
            Revision = -1,
            PackageId = 1
        };

        [OneTimeSetUp]
        public void Init()
        {
            packageRepositoryMock = new Mock<IRepository<Package>>();
            nugetApiServiceMock = new Mock<INugetApiService>();
            versionServiceMock = new Mock<IVersionService>();
            uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(p => p.GetGenericRepository<Package>())
                .Returns(packageRepositoryMock.Object);

            nugetService = new NugetService(nugetApiServiceMock.Object, versionServiceMock.Object, uowMock.Object);
        }

        [Test]
        public async Task RefreshLatestVersionOfAllPackagesAsync_Should_Invokes_UpdateLatestVersionOfPackagesAsync_When_Valid_Values()
        {
            packageRepositoryMock
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(GetListPackage);

            nugetApiServiceMock
                .Setup(p => p.GetLatestVersionPackageAsync(It.IsAny<string>()))
                .ReturnsAsync(testVersion);

            await nugetService.RefreshLatestVersionOfAllPackagesAsync();

            versionServiceMock.Verify(x => x.UpdateLatestVersionOfPackagesAsync(It.IsAny<IEnumerable<PackageVersion>>()));
        }

        [Test]
        public async Task RefreshLatestVersionOfNewPackagesAsync_Should_Invokes_UpdateLatestVersionOfNewPackagesAsync_When_Valid_Values()
        {
            packageRepositoryMock
                .Setup(p => p.GetAsync(It.IsAny<Expression<Func<Package, bool>>>()))
                .ReturnsAsync(GetListPackage);

            nugetApiServiceMock
                .Setup(p => p.GetLatestVersionPackageAsync(It.IsAny<string>()))
                .ReturnsAsync(testVersion);
            
            await nugetService.RefreshLatestVersionOfNewPackagesAsync();

            versionServiceMock.Verify(x => x.UpdateLatestVersionOfNewPackagesAsync(It.IsAny<IEnumerable<PackageVersion>>()));
        }

        private List<Package> GetListPackage()
        {
            return new List<Package>
            {
                new Package
                {
                    Id = 1,
                    Name = "NUnit",
                    Versions = new List<PackageVersion>
                    {
                        new PackageVersion
                        {
                            Id = 1,
                            Major = 1,
                            Minor = 1,
                            Build = 1,
                            Revision = -1,
                            PackageId = 1
                        }
                    },
                    LastUpdateTime = null
                },

                new Package
                {
                    Id = 2,
                    Name = "Moq",
                    Versions = new List<PackageVersion>
                    {
                        new PackageVersion
                        {
                            Id = 2,
                            Major = 4,
                            Minor = 10,
                            Build = 0,
                            Revision = -1,
                            PackageId = 2
                        }
                    },
                    LastUpdateTime = null
                },
            };
        }
    }
}