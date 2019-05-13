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
        private Mock<IRepository<Package>> packageRepository;
        private PackageService packageService;

        [OneTimeSetUp]
        public void Init()
        {
            uowMock = new Mock<IUnitOfWork>();
            packageRepository = new Mock<IRepository<Package>>();

            packageRepository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(It.IsAny<IReadOnlyCollection<Package>>());

            uowMock
                .Setup(uow => uow.GetRepository<Package>())
                .Returns(packageRepository.Object);

            packageService = new PackageService(uowMock.Object);
        }

        [Test]
        public async Task GetAllAsync_Should_Invoke_GetAllAsync()
        {
            await packageService.GetAllAsync();

            packageRepository.Verify(p => p.GetAllAsync());
        }

        [Test]
        public async Task GetNewPackagesAsync_Should_Invoke_GetAsync()
        {
            packageRepository
                .Setup(p => p.GetAsync(It.IsAny<Expression<Func<Package, bool>>>()))
                .ReturnsAsync(It.IsAny<IReadOnlyCollection<Package>>());

            await packageService.GetNewlyAddedPackagesAsync();

            packageRepository.Verify(p => p.GetAsync(It.IsAny<Expression<Func<Package, bool>>>()));
        }
    }
}