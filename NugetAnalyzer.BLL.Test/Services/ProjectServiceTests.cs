using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.DTOs.Models.Reports;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture]
    public class ProjectServiceTests
    {
        private readonly PackageVersion nUnitPackageVersion;
        private readonly PackageVersion nUnitLatestPackageVersion;
        private readonly PackageVersion moqPackageVersion;
        private readonly PackageVersion moqLatestPackageVersion;
        private readonly List<Project> projects;
        private readonly List<PackageVersion> packageVersions;
        private readonly List<PackageVersion> latestPackageVersions;
        private readonly List<Package> packages;
        private readonly PackageVersionComparisonReport nUnitReport;
        private readonly PackageVersionComparisonReport moqReport;
        private readonly PackageVersionComparisonReport projectReport;
        private readonly List<PackageVersionComparisonReport> reports;
       
        private Mock<IUnitOfWork> uowMock;
        private Mock<IVersionsAnalyzerService> versionAnalyzerServiceMock;
        private Mock<IProjectsRepository> projectsRepositoryMock;
        private Mock<IPackageVersionsRepository> packageVersionsRepositoryMock;

        private ProjectService projectService;

        public ProjectServiceTests()
        {
            nUnitPackageVersion = new PackageVersion
            {
                Id = 1,
                Major = 1,
                Minor = 1,
                Build = -1,
                Revision = -1,
                PublishedDate = DateTime.Now.AddMonths(-5),
                PackageId = 1
            };

            moqPackageVersion = new PackageVersion
            {
                Id = 2,
                Major = 2,
                Minor = 2,
                Build = 2,
                Revision = -1,
                PublishedDate = DateTime.Now.AddMonths(-10),
                PackageId = 2
            };

            nUnitLatestPackageVersion = new PackageVersion
            {
                Id = 3,
                Major = 2,
                Minor = 1,
                Build = -1,
                Revision = -1,
                PublishedDate = DateTime.Now,
                PackageId = 1
            };

            moqLatestPackageVersion = new PackageVersion
            {
                Id = 4,
                Major = 3,
                Minor = 2,
                Build = 2,
                Revision = -1,
                PublishedDate = DateTime.Now,
                PackageId = 2
            };

            var nUnitPackage = new Package
            {
                Id = 1,
                Name = "NUnit",
                Versions = new List<PackageVersion>
                {
                    nUnitPackageVersion,
                },
                LastUpdateTime = null
            };

            var moqPackage = new Package
            {
                Id = 2,
                Name = "Moq",
                Versions = new List<PackageVersion>
                {
                    moqPackageVersion
                },
                LastUpdateTime = null
            };

            nUnitPackageVersion.Package = nUnitPackage;

            moqPackageVersion.Package = moqPackage;

            packageVersions = new List<PackageVersion>
            {
                nUnitPackageVersion,
                moqPackageVersion
            };

            latestPackageVersions = new List<PackageVersion>
            {
                nUnitLatestPackageVersion,
                moqLatestPackageVersion
            };

            packages = new List<Package>
            {
                nUnitPackage,
                moqPackage
            };

            var projectPackageVersions = new List<ProjectPackageVersion>
            {
                new ProjectPackageVersion
                {
                    PackageVersionId = 1,
                    PackageVersion = nUnitPackageVersion,
                    ProjectId = 1,
                    
                },
                new ProjectPackageVersion
                {
                    PackageVersionId = 2,
                    PackageVersion = moqPackageVersion,
                    ProjectId = 1,
                }
            };

            projects = new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Name = "TestProject",
                    ProjectPackageVersions = projectPackageVersions
                }
            };

            nUnitReport = new PackageVersionComparisonReport();

            moqReport = new PackageVersionComparisonReport();

            projectReport = new PackageVersionComparisonReport();

            reports = new List<PackageVersionComparisonReport>
            {
                nUnitReport,
                moqReport
            };
    }

        [OneTimeSetUp]
        public void Init()
        {
            uowMock = new Mock<IUnitOfWork>();
            projectsRepositoryMock = new Mock<IProjectsRepository>();
            packageVersionsRepositoryMock = new Mock<IPackageVersionsRepository>();
            versionAnalyzerServiceMock = new Mock<IVersionsAnalyzerService>();

            projectsRepositoryMock
                .Setup(projectsRepository => projectsRepository.GetCollectionIncludePackageAsync(It.IsAny<Expression<Func<Project, bool>>>()))
                .ReturnsAsync(projects);

            packageVersionsRepositoryMock
                .Setup(packageVersionsRepository => 
                    packageVersionsRepository.GetLatestVersionsAsync(It.IsAny<Expression<Func<PackageVersion, bool>>>()))
                .ReturnsAsync(latestPackageVersions);

            uowMock
                .Setup(uow => uow.ProjectsRepository)
                .Returns(projectsRepositoryMock.Object);

            uowMock
                .Setup(uow => uow.PackageVersionsRepository)
                .Returns(packageVersionsRepositoryMock.Object);

            versionAnalyzerServiceMock
                .Setup(versionAnalyzerService => versionAnalyzerService.Compare(nUnitLatestPackageVersion, nUnitPackageVersion))
                .Returns(nUnitReport);

            versionAnalyzerServiceMock
                .Setup(versionAnalyzerService => versionAnalyzerService.Compare(moqLatestPackageVersion, moqPackageVersion))
                .Returns(moqReport);

            versionAnalyzerServiceMock
                .Setup(versionAnalyzerService => versionAnalyzerService.CalculateMaxReportLevelStatus(reports))
                .Returns(projectReport);

            projectService = new ProjectService(uowMock.Object, versionAnalyzerServiceMock.Object);
        }

        [Test]
        public async Task GetProjectReport_Should_Invoke_CalculateMaxReportLevelStatus()
        {
            PackageVersion packageVersion = new PackageVersion
            {
                Id = 1
            };

            ProjectReportDTO result = await projectService.GetProjectReportAsync(1);

            packageVersionsRepositoryMock.Verify(packageVersionsRepository =>
                packageVersionsRepository.GetLatestVersionsAsync(It.IsAny<Expression<Func<PackageVersion, bool>>>()));

            projectsRepositoryMock.Verify(projectsRepository => 
                projectsRepository.GetCollectionIncludePackageAsync(It.IsAny<Expression<Func<Project, bool>>>()));

            versionAnalyzerServiceMock.Verify(versionAnalyzerService =>
                versionAnalyzerService.Compare(nUnitLatestPackageVersion, nUnitPackageVersion));

            versionAnalyzerServiceMock.Verify(versionAnalyzerService =>
                versionAnalyzerService.Compare(moqLatestPackageVersion, moqPackageVersion));

            versionAnalyzerServiceMock.Verify(versionAnalyzerService =>
                versionAnalyzerService.CalculateMaxReportLevelStatus(reports));

            Assert.IsInstanceOf<ProjectReportDTO>(result);
            Assert.NotNull(result);
        }
    }
}