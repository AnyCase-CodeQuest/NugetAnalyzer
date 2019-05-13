using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Models.Enums;
using NugetAnalyzer.BLL.Models.Projects;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.BLL.Models.Solutions;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class RepositoryServiceTests
    {
        private Mock<IVersionAnalyzerService> versionService;
        private Mock<IRepositoryRepository> repositoryRepository;
        private Mock<IVersionRepository> versionRepository;

        private RepositoryService repositoryService;

        private readonly Expression<Func<Repository, bool>> expression = r => r.UserId == 1;

        private readonly PackageVersionComparisonReport report = new PackageVersionComparisonReport
        {
            IsObsolete = true,
            VersionStatus = PackageVersionStatus.Info,
            DateStatus = PackageDateStatus.Warning
        };

        [OneTimeSetUp]
        public void Init()
        {
            versionService = new Mock<IVersionAnalyzerService>();
            repositoryRepository = new Mock<IRepositoryRepository>();
            versionRepository = new Mock<IVersionRepository>();

            repositoryService = new RepositoryService(versionService.Object, repositoryRepository.Object, versionRepository.Object);
        }

        [Test]
        public void Constructor_Check_AllNullArgumentsThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositoryService(null, repositoryRepository.Object, versionRepository.Object));
            Assert.Throws<ArgumentNullException>(() => new RepositoryService(versionService.Object, null, versionRepository.Object));
            Assert.Throws<ArgumentNullException>(() => new RepositoryService(versionService.Object, repositoryRepository.Object, null));
        }

        [Test]
        public async Task GetAnalyzedRepositoriesAsync_Check_AllMethodsUsedWithProperParameters()
        {
            repositoryRepository
                .Setup(r => r.GetRepositoriesWithIncludesAsync(It.IsAny<Expression<Func<Repository, bool>>>()))
                .ReturnsAsync(GetRepositories());
            versionRepository
                .Setup(r => r.GetLatestPackageVersionsAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(GetLatestPackageVersions());

            await repositoryService.GetAnalyzedRepositoriesAsync(expression);

            repositoryRepository.Verify(r => r.GetRepositoriesWithIncludesAsync(expression), Times.Once());
            versionRepository.Verify(r => r.GetLatestPackageVersionsAsync(It.IsAny<ICollection<int>>()), Times.Once());
        }

        [Test]
        public async Task GetAnalyzedRepositoriesAsync_CheckReturnsValue()
        {
            repositoryRepository
                .Setup(r => r.GetRepositoriesWithIncludesAsync(It.IsAny<Expression<Func<Repository, bool>>>()))
                .ReturnsAsync(GetRepositories());
            versionRepository
                .Setup(r => r.GetLatestPackageVersionsAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(GetLatestPackageVersions());
            versionService.Setup(s => s.Compare(It.IsAny<PackageVersion>(), It.IsAny<PackageVersion>()))
                .Returns(report);
            versionService
                .Setup(s => s.CalculateMaxReportLevelStatus(It.IsAny<ICollection<PackageVersionComparisonReport>>()))
                .Returns(report);

            var result = await repositoryService.GetAnalyzedRepositoriesAsync(expression);

            Assert.IsTrue(result.IsDeepEqual(GetAnalyzedRepositories()));
        }

        private IReadOnlyCollection<PackageVersion> GetLatestPackageVersions()
        {
            return new List<PackageVersion>
            {
                new PackageVersion
                {
                    Major = 1,
                    Minor = 1,
                    Build = 5,
                    Revision = 5,
                    PublishedDate = new DateTime(2019, 1, 5),
                    PackageId = 1,
                },
                new PackageVersion
                {
                    Major = 2,
                    Minor = 2,
                    Build = 3,
                    Revision = 1,
                    PublishedDate = new DateTime(2018, 1, 5),
                    PackageId = 2,
                },
                new PackageVersion
                {
                    Major = 4,
                    Minor = 4,
                    Build = 4,
                    Revision = 4,
                    PublishedDate = new DateTime(2019, 1, 5),
                    PackageId = 3
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

        private ICollection<RepositoryWithVersionReport> GetAnalyzedRepositories()
        {
            return new List<RepositoryWithVersionReport>
            {
                new RepositoryWithVersionReport
                {
                    Report = report,
                    Solutions = new List<SolutionWithVersionReport>
                    {
                        new SolutionWithVersionReport
                        {
                            Report = report,
                            Projects = new List<ProjectWithVersionReport>
                            {
                                new ProjectWithVersionReport
                                {
                                    Report = report
                                },
                                new ProjectWithVersionReport
                                {
                                    Report = report
                                },
                            }
                        },
                        new SolutionWithVersionReport
                        {
                            Report = report,
                            Projects = new List<ProjectWithVersionReport>
                            {
                                new ProjectWithVersionReport
                                {
                                    Report = report
                                },
                                new ProjectWithVersionReport
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
