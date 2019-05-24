using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture]
    public class PackageServiceTests
    {
        private Mock<IUnitOfWork> uowMock;
        private Mock<IPackagesRepository> packageRepository;
        private PackageService packageService;

        [OneTimeSetUp]
        public void Init()
        {
            uowMock = new Mock<IUnitOfWork>();
            packageRepository = new Mock<IPackagesRepository>();

            packageRepository
                .Setup(packageRepository => packageRepository.GetAllAsync())
                .ReturnsAsync(It.IsAny<IReadOnlyCollection<Package>>());

            uowMock
                .Setup(uow => uow.PackagesRepository)
                .Returns(packageRepository.Object);

            packageService = new PackageService(uowMock.Object);
        }

        [Test]
        public async Task GetAllAsync_Should_Invoke_GetAllAsync()
        {
            await packageService.GetAllAsync();

            packageRepository.Verify(packageRepository => packageRepository.GetAllAsync());
        }

        [Test]
        public async Task GetNewPackagesAsync_Should_Invoke_GetAsync()
        {
            packageRepository
                .Setup(packageRepository => packageRepository.GetIncludePackageVersionWithNotSetPublishedDateAsync())
                .ReturnsAsync(It.IsAny<IReadOnlyCollection<Package>>());

            await packageService.GetPackagesOfNewlyAddedPackageVersionsAsync();

            packageRepository.Verify(packageRepository => packageRepository.GetIncludePackageVersionWithNotSetPublishedDateAsync());
        }
    }
}