using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class RepositorySaverServiceTests
    {
        private const int UserId = 1;
        private const int NullUserId = 0;
        private const string VersionString = "1.1.1.1";
        private const string FirstPackageName = "TestPackage";
        private const string SecondPackageName = "AnotherTestPackage";
        private const string RepositoryName = "TestRepository";
        private const string SolutionName = "TestSolution";
        private const string FirstProjectName = "TestProject";
        private const string SecondProjectName = "SecondTestProject";
        private readonly RepositoryDTO nullRepositoryDTO = null;
        private RepositoryDTO accurateRepositoryDTO;
        private readonly IUnitOfWork NullUnitOfWork = null;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IRepository<Repository>> databaseRepositoryMock;
        private Mock<IRepository<PackageVersion>> packageVersionRepositoryMock;
        private Mock<IRepository<Package>> packageRepositoryMock;
        private IRepositorySaverService repositoryService;

        [SetUp]
        public void SetUp()
        {
            databaseRepositoryMock = new Mock<IRepository<Repository>>();
            packageVersionRepositoryMock = new Mock<IRepository<PackageVersion>>();
            packageRepositoryMock = new Mock<IRepository<Package>>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(UnitOfWork => UnitOfWork.GetRepository<Repository>())
                .Returns(databaseRepositoryMock.Object);
            unitOfWorkMock.Setup(UnitOfWork => UnitOfWork.GetRepository<PackageVersion>())
                .Returns(packageVersionRepositoryMock.Object);
            unitOfWorkMock.Setup(UnitOfWork => UnitOfWork.GetRepository<Package>())
                .Returns(packageRepositoryMock.Object);
            repositoryService = new RepositorySaverService(unitOfWorkMock.Object);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            accurateRepositoryDTO = new RepositoryDTO
            {
                Name = RepositoryName,
                Solutions = new List<SolutionDTO>
                {
                    new SolutionDTO
                    {
                        Name = SolutionName,
                        Projects = new List<ProjectDTO>
                        {
                            new ProjectDTO
                            {
                                Name = FirstProjectName,
                                Packages = new List<PackageDTO>
                                {
                                    new PackageDTO
                                    {
                                        Name = FirstPackageName,
                                        Version = VersionString
                                    },
                                    new PackageDTO
                                    {
                                        Name = SecondPackageName,
                                        Version = VersionString
                                    }
                                }
                            },
                            new ProjectDTO
                            {
                                Name = SecondProjectName,
                                Packages = new List<PackageDTO>
                                {
                                    new PackageDTO
                                    {
                                        Name = FirstPackageName,
                                        Version = VersionString
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        [Test]
        public void SaveAsync_ShouldThrowNullReferenceException_WhenInputDataIsNull()
        {
            Assert.ThrowsAsync<NullReferenceException>(() => repositoryService.SaveAsync(nullRepositoryDTO, NullUserId));
        }

        [Test]
        public void RepositoryServiceInitialization_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => repositoryService = new RepositorySaverService(NullUnitOfWork));
        }

        [Test]
        public void SaveAsync_ShouldNotThrowException_WhenRepositoryIsAccurate()
        {
            Assert.DoesNotThrowAsync(() => repositoryService.SaveAsync(accurateRepositoryDTO, UserId));
        }

    }
}