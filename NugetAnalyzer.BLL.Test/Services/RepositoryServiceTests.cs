using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models.Enums;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class RepositoryServiceTests
    {
        private Mock<IVersionsAnalyzerService> versionAnalyzerServiceMock;
        private Mock<IRepositoriesRepository> repositoryRepositoryMock;
        private Mock<IPackageVersionsRepository> versionRepositoryMock;
        private Mock<IUnitOfWork> uowMock;

        private RepositoryService repositoryService;

        private readonly Expression<Func<Repository, bool>> expression = repository => repository.UserId == 1;

        private readonly PackageVersionComparisonReport report = new PackageVersionComparisonReport
        {
            IsObsolete = true,
            VersionStatus = PackageVersionStatus.Info,
            DateStatus = PackageDateStatus.Warning
        };

        [OneTimeSetUp]
        public void Init()
        {
            versionAnalyzerServiceMock = new Mock<IVersionsAnalyzerService>();
            repositoryRepositoryMock = new Mock<IRepositoriesRepository>();
            versionRepositoryMock = new Mock<IPackageVersionsRepository>();
            uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(uow => uow.RepositoriesRepository).Returns(repositoryRepositoryMock.Object);
            uowMock.SetupGet(uow => uow.PackageVersionsRepository).Returns(versionRepositoryMock.Object);

            repositoryService = new RepositoryService(versionAnalyzerServiceMock.Object, uowMock.Object);
        }

        [Test]
        public void Constructor_Should_ThrowsArgumentNullException_When_AnyArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositoryService(null, uowMock.Object));
            Assert.Throws<ArgumentNullException>(() => new RepositoryService(versionAnalyzerServiceMock.Object, null));
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

            var result = await repositoryService.GetAnalyzedRepositoriesAsync(expression);

            Assert.IsTrue(result.IsDeepEqual(GetAnalyzedRepositories()));
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
                        PublishedDate = new DateTime(2019, 1, 5),
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
                        PublishedDate = new DateTime(2018, 1, 5),
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
                        PublishedDate = new DateTime(2019, 1, 5),
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
                                                PublishedDate = new DateTime(2019, 5, 5),
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
                                                PublishedDate = new DateTime(2017, 5, 13),
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
                                                PublishedDate = new DateTime(2019, 5, 5),
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
                                                PublishedDate = new DateTime(2017, 1, 5),
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
                                                PublishedDate = new DateTime(2019, 5, 5),
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
                                                PublishedDate = new DateTime(2017, 1, 5),
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

        private ICollection<RepositoryVersionReport> GetAnalyzedRepositories()
        {
            return new List<RepositoryVersionReport>
            {
                new RepositoryVersionReport
                {
                    Report = report,
                    Solutions = new List<SolutionVersionReport>
                    {
                        new SolutionVersionReport
                        {
                            Report = report,
                            Projects = new List<ProjectVersionReport>
                            {
                                new ProjectVersionReport
                                {
                                    Report = report
                                },
                                new ProjectVersionReport
                                {
                                    Report = report
                                },
                            }
                        },
                        new SolutionVersionReport
                        {
                            Report = report,
                            Projects = new List<ProjectVersionReport>
                            {
                                new ProjectVersionReport
                                {
                                    Report = report
                                },
                                new ProjectVersionReport
                                {
                                    Report = report
                                },
                            }
                        }
                    }
                }
            };
        }
    }
}
