using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using Project = NugetAnalyzer.BLL.Models.Projects.Project;
using Solution = NugetAnalyzer.BLL.Models.Solutions.Solution;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class RepositoryServiceTests
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
        private readonly Models.Repositories.Repository NullRepository = null;
        private Models.Repositories.Repository AccurateRepository;
        private readonly IUnitOfWork NullUnitOfWork = null;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IRepository<Repository>> databaseRepositoryMock;
        private Mock<IRepository<PackageVersion>> packageVersionRepositoryMock;
        private Mock<IRepository<Package>> packageRepositoryMock;
        private IRepositoryService repositoryService;

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
            repositoryService = new RepositoryService(unitOfWorkMock.Object);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            AccurateRepository = new Models.Repositories.Repository
            {
                Name = RepositoryName,
                Solutions = new List<Solution>
                {
                    new Solution
                    {
                        Name = SolutionName,
                        Projects = new List<Project>
                        {
                            new Project
                            {
                                Name = FirstProjectName,
                                Packages = new List<Models.Packages.Package>
                                {
                                    new Models.Packages.Package
                                    {
                                        Name = FirstPackageName,
                                        Version = VersionString
                                    },
                                    new Models.Packages.Package
                                    {
                                        Name = SecondPackageName,
                                        Version = VersionString
                                    }
                                }
                            },
                            new Project
                            {
                                Name = SecondProjectName,
                                Packages = new List<Models.Packages.Package>
                                {
                                    new Models.Packages.Package
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
            Assert.ThrowsAsync<NullReferenceException>(() => repositoryService.SaveAsync(NullRepository, NullUserId));
        }

        [Test]
        public void RepositoryServiceInitialization_ShouldThrowArgumentNullException_WhenUnitOfWorkIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => repositoryService = new RepositoryService(NullUnitOfWork));
        }

        [Test]
        public void SaveAsync_ShouldNotThrowException_WhenRepositoryIsAccurate()
        {
            Assert.DoesNotThrowAsync(() => repositoryService.SaveAsync(AccurateRepository, UserId));
        }

    }
}