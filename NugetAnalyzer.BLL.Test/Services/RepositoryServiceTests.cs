using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common.Interfaces;
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
        private Models.Repositories.Repository NullRepository = null;
        private Models.Repositories.Repository AccurateRepository = new Models.Repositories.Repository
        {
            Name = "TestRepository",
            Solutions = new List<Solution>
            {
                new Solution
                {
                    Name = "TestSolution",
                    Projects = new List<Project>
                    {
                        new Project
                        {
                            Name = "TestProject1",
                            Packages = new List<Models.Packages.Package>
                            {
                                new Models.Packages.Package
                                {
                                    Name = "TestPackage",
                                    Version = "1.1.1.1"
                                },
                                new Models.Packages.Package
                                {
                                    Name="AnotherTestPackage",
                                    Version = "1.1.1.1"
                                }
                            }
                        },
                        new Project
                        {
                            Name="TestProject2",
                            Packages=new List<Models.Packages.Package>
                            {
                                new Models.Packages.Package
                                {
                                    Name = "TestPackage",
                                    Version = "1.1.1.1"
                                }
                            }
                        }
                    }
                }
            }
        };
        private IUnitOfWork NullUnitOfWork = null;

        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IRepository<Repository>> databaseRepositoryMock;
        private Mock<IRepository<PackageVersion>> packageVersionRepositoryMock;
        private Mock<IRepository<Package>> packageRepositoryMock;
        private IRepositoryService repositoryService;

        [SetUp]
        public void Init()
        {
            databaseRepositoryMock = new Mock<IRepository<Repository>>();
            packageVersionRepositoryMock = new Mock<IRepository<PackageVersion>>();
            packageRepositoryMock = new Mock<IRepository<Package>>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(UnitOfWork => UnitOfWork.GetRepository<Repository>()).Returns(databaseRepositoryMock.Object);
            unitOfWorkMock.Setup(UnitOfWork => UnitOfWork.GetRepository<PackageVersion>())
                .Returns(packageVersionRepositoryMock.Object);
            unitOfWorkMock.Setup(UnitOfWork => UnitOfWork.GetRepository<Package>()).Returns(packageRepositoryMock.Object);
            repositoryService = new RepositoryService(unitOfWorkMock.Object);
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
            databaseRepositoryMock.Setup(Repository =>
                Repository.GetSingleOrDefaultAsync(Rep => Rep.Name == "TestRepository")).Returns(Task.FromResult(new Repository()));
            packageRepositoryMock
                .Setup(dbPackage => dbPackage.GetSingleOrDefaultAsync(Rep => Rep.Name == "AnotherTestPackage"))
                .Returns(Task.FromResult(new Package()));

            Assert.DoesNotThrowAsync(() => repositoryService.SaveAsync(AccurateRepository, UserId));
        }

    }
}