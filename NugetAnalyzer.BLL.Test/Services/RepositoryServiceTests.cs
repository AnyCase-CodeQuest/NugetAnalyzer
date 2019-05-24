using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Microsoft.Extensions.Logging;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Models.Enums;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.DTOs.Models.Reports;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class RepositoryServiceTests
    {
        private Mock<IVersionsAnalyzerService> versionAnalyzerServiceMock;
        private Mock<IRepositoriesRepository> repositoryRepositoryMock;
        private Mock<IPackageVersionsRepository> versionRepositoryMock;
        private Mock<IGitService> gitServiceMock;
        private Mock<IRepositoryAnalyzerService> repositoryAnalyzerServiceMock;
        private Mock<IRepositorySaverService> repositorySaverServiceMock;
        private Mock<INugetService> nugetServiceMock;
        private Mock<IDirectoryService> directoryServiceMock;
        private Mock<ILogger<RepositoryService>> loggerMock;
        private Mock<IUnitOfWork> uowMock;

        private RepositoryService repositoryService;

        private readonly Expression<Func<Repository, bool>> expression = repository => repository.UserId == 1;

        private readonly PackageVersionComparisonReport report = new PackageVersionComparisonReport
        {
            IsObsolete = true,
            VersionStatus = PackageVersionStatus.Info,
            DateStatus = PackageVersionStatus.Warning
        };

        [OneTimeSetUp]
        public void Init()
        {
            versionAnalyzerServiceMock = new Mock<IVersionsAnalyzerService>();
            repositoryRepositoryMock = new Mock<IRepositoriesRepository>();
            versionRepositoryMock = new Mock<IPackageVersionsRepository>();
            gitServiceMock = new Mock<IGitService>();
            repositoryAnalyzerServiceMock = new Mock<IRepositoryAnalyzerService>();
            repositorySaverServiceMock = new Mock<IRepositorySaverService>();
            nugetServiceMock = new Mock<INugetService>();
            directoryServiceMock = new Mock<IDirectoryService>();
            loggerMock = new Mock<ILogger<RepositoryService>>();
            uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(uow => uow.RepositoriesRepository).Returns(repositoryRepositoryMock.Object);
            uowMock.SetupGet(uow => uow.PackageVersionsRepository).Returns(versionRepositoryMock.Object);

            repositoryService = new RepositoryService(versionAnalyzerServiceMock.Object,
                gitServiceMock.Object,
                repositoryAnalyzerServiceMock.Object,
                repositorySaverServiceMock.Object,
                nugetServiceMock.Object,
                directoryServiceMock.Object,
                loggerMock.Object,
                uowMock.Object);
        }

        [Test]
        public async Task GetAnalyzedRepositoriesAsync_Check_AllMethodsUsedWithProperParameters()
        {
            repositoryRepositoryMock
                .Setup(repositoryRepository => repositoryRepository.GetRepositoriesWithIncludesAsync(It.IsAny<Expression<Func<Repository, bool>>>()))
                .ReturnsAsync(GetRepositories());
            versionRepositoryMock
                .Setup(versionRepository => versionRepository.GetLatestPackageVersionsAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(GetLatestPackageVersions());

            await repositoryService.GetAnalyzedRepositoriesAsync(expression);

            repositoryRepositoryMock.Verify(repository => repository.GetRepositoriesWithIncludesAsync(expression), Times.Once());
            versionRepositoryMock.Verify(repository => repository.GetLatestPackageVersionsAsync(It.IsAny<ICollection<int>>()), Times.Once());
        }

        [Test]
        public async Task GetAnalyzedRepositoriesAsync_Check_ReturnsValue()
        {
            repositoryRepositoryMock
                .Setup(repositoryRepository => repositoryRepository.GetRepositoriesWithIncludesAsync(It.IsAny<Expression<Func<Repository, bool>>>()))
                .ReturnsAsync(GetRepositories());
            versionRepositoryMock
                .Setup(versionRepository => versionRepository.GetLatestPackageVersionsAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(GetLatestPackageVersions());
            versionAnalyzerServiceMock.Setup(versionAnalyzerService => versionAnalyzerService.Compare(It.IsAny<PackageVersion>(), It.IsAny<PackageVersion>()))
                .Returns(report);
            versionAnalyzerServiceMock
                .Setup(versionAnalyzerService => versionAnalyzerService.CalculateMaxReportLevelStatus(It.IsAny<ICollection<PackageVersionComparisonReport>>()))
                .Returns(report);

            ICollection<RepositoryReport> result = await repositoryService.GetAnalyzedRepositoriesAsync(expression);

            Assert.IsTrue(result.IsDeepEqual(GetAnalyzedRepositories()));
        }

        [Test]
        public async Task GetRepositoriesNamesAsync_Check_AllMethodsUsedWithProperParameters()
        {
            var repositoriesNames = new List<string>(0);

            repositoryRepositoryMock
                .Setup(repositoryRepository => repositoryRepository.GetRepositoriesNamesAsync(It.IsAny<Expression<Func<Repository, bool>>>()))
                .ReturnsAsync(repositoriesNames);

            IReadOnlyCollection<string> result = await repositoryService.GetRepositoriesNamesAsync(expression);

            Assert.AreEqual(repositoriesNames, result);
            repositoryRepositoryMock.Verify(repository => repository.GetRepositoriesNamesAsync(expression), Times.Once);
        }

        [Test]
        public async Task AddRepositoriesAsync_Check_AllMethodsUsedWithProperParameters()
        {
            var repositoriesForAdd = GetRepositoriesForAdd();
            var userToken = "userToken";
            var userId = 1;
            var clonePath = "clonePath";
            var repositoryName = "repositoryName";
            var parsedRepository = new RepositoryDTO();

            directoryServiceMock
                .Setup(directoryService => directoryService.GenerateClonePath())
                .Returns(clonePath);
            directoryServiceMock
                .Setup(directoryService => directoryService.GetName(It.IsAny<string>()))
                .Returns(repositoryName);
            repositoryAnalyzerServiceMock
                .Setup(repositoryAnalyzerService => repositoryAnalyzerService.GetParsedRepositoryAsync(It.IsAny<string>()))
                .ReturnsAsync(parsedRepository);

            var result = await repositoryService.AddRepositoriesAsync(repositoriesForAdd, userToken, userId);

            directoryServiceMock.Verify(directoryService => directoryService.GenerateClonePath());
            directoryServiceMock.Verify(directoryService => directoryService.GetName(It.IsAny<string>()));

            gitServiceMock.Verify(gitService => gitService.CloneBranch(
                    It.Is<string>(match => repositoriesForAdd.ContainsKey(match)),
                    $"{clonePath}/{repositoryName}",
                    userToken,
                    It.Is<string>(match => repositoriesForAdd.ContainsValue(match))));

            repositoryAnalyzerServiceMock.Verify(repositoryAnalyzerService =>
                repositoryAnalyzerService.GetParsedRepositoryAsync($"{clonePath}/{repositoryName}"));

            repositorySaverServiceMock.Verify(repositorySaverService =>
                repositorySaverService.SaveAsync(parsedRepository, userId));

            nugetServiceMock.Verify(nugetService => nugetService.RefreshNewlyAddedPackageVersionsAsync());

            directoryServiceMock.Verify(directoryService => directoryService.Delete(clonePath));
        }

        private Dictionary<int, PackageVersion> GetLatestPackageVersions()
        {
            return new Dictionary<int, PackageVersion>
            {
                {
                    1,
                    new PackageVersion
                    {
                        Major = 1,
                        Minor = 1,
                        Build = 5,
                        Revision = 5,
                        PublishedDate =  DateTime.Now.AddMonths(-4),
                        PackageId = 1,
                    }
                },
                {
                    2,
                    new PackageVersion
                    {
                        Major = 2,
                        Minor = 2,
                        Build = 3,
                        Revision = 1,
                        PublishedDate =  DateTime.Now.AddYears(-1).AddMonths(-4),
                        PackageId = 2,
                    }
                },
                {
                    3,
                    new PackageVersion
                    {
                        Major = 4,
                        Minor = 4,
                        Build = 4,
                        Revision = 4,
                        PublishedDate =  DateTime.Now.AddMonths(-4),
                        PackageId = 3
                    }
                }
            };
        }

        private IReadOnlyCollection<Repository> GetRepositories()
        {
            return new List<Repository>
            {
                new Repository
                {
                    Solutions = new List<Solution>
                    {
                        new Solution
                        {
                            Projects = new List<Project>()
                            {
                                new Project
                                {
                                    ProjectPackageVersions = new List<ProjectPackageVersion>
                                    {
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 1,
                                                Minor = 1,
                                                Build = 1,
                                                Revision = 1,
                                                PublishedDate =  DateTime.Now,
                                                PackageId = 1
                                            }
                                        },
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 2,
                                                Minor = 2,
                                                Build = 2,
                                                Revision = 2,
                                                PublishedDate =  DateTime.Now.AddYears(-2),
                                                PackageId = 2
                                            }
                                        }
                                    }
                                },
                                new Project
                                {
                                    ProjectPackageVersions = new List<ProjectPackageVersion>
                                    {
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 3,
                                                Minor = 3,
                                                Build = 3,
                                                Revision = 3,
                                                PublishedDate =  DateTime.Now,
                                                PackageId = 3
                                            }
                                        },
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 1,
                                                Minor = 1,
                                                Build = 1,
                                                Revision = 1,
                                                PublishedDate =  DateTime.Now.AddYears(-2).AddMonths(-4),
                                                PackageId = 1
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new Solution
                        {
                            Projects = new List<Project>()
                            {
                                new Project
                                {
                                    ProjectPackageVersions = new List<ProjectPackageVersion>
                                    {
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 1,
                                                Minor = 1,
                                                Build = 2,
                                                Revision = 2,
                                                PublishedDate =  DateTime.Now,
                                                PackageId = 1
                                            }
                                        },
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 2,
                                                Minor = 3,
                                                Build = 3,
                                                Revision = 3,
                                                PackageId = 2
                                            }
                                        }
                                    }
                                },
                                new Project
                                {
                                    ProjectPackageVersions = new List<ProjectPackageVersion>
                                    {
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 4,
                                                Minor = 4,
                                                Build = 3,
                                                Revision = 3,
                                                PackageId = 3
                                            }
                                        },
                                        new ProjectPackageVersion
                                        {
                                            PackageVersion = new PackageVersion
                                            {
                                                Major = 1,
                                                Minor = 1,
                                                Build = 1,
                                                Revision = 1,
                                                PublishedDate = DateTime.Now.AddYears(-2).AddMonths(-4),
                                                PackageId = 1
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private ICollection<RepositoryReport> GetAnalyzedRepositories()
        {
            return new List<RepositoryReport>
            {
                new RepositoryReport
                {
                    Report = report,
                    Solutions = new List<SolutionReport>
                    {
                        new SolutionReport
                        {
                            Report = report,
                            Projects = new List<ProjectReport>
                            {
                                new ProjectReport
                                {
                                    Report = report
                                },
                                new ProjectReport
                                {
                                    Report = report
                                },
                            }
                        },
                        new SolutionReport
                        {
                            Report = report,
                            Projects = new List<ProjectReport>
                            {
                                new ProjectReport
                                {
                                    Report = report
                                },
                                new ProjectReport
                                {
                                    Report = report
                                },
                            }
                        }
                    }
                }
            };
        }

        private Dictionary<string, string> GetRepositoriesForAdd()
        {
            return new Dictionary<string, string>
            {
                { "https://github.com/1...", "master" },
                { "https://github.com/2...", "develop" },
                { "https://github.com/3...", "feature/20" },
            };
        }
    }
}
