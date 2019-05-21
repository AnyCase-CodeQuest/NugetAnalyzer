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
        private readonly PackageVersion nUnitVersion;
        private readonly PackageVersion nUnitLatestVersion;
        private readonly PackageVersion moqVersion;
        private readonly PackageVersion moqLatestVersion;
        private readonly Package nUnitPackage;
        private readonly Package moqPackage;
        private readonly List<ProjectPackageVersion> projectPackageVersions;
        private readonly Project project;
        private readonly List<Project> projects;
        private readonly List<PackageVersion> packageVersions;
        private readonly List<PackageVersion> packageLatestVersions;
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
            nUnitVersion = new PackageVersion
            {
                Id = 1,
                Major = 1,
                Minor = 1,
                Build = -1,
                Revision = -1,
                PublishedDate = DateTime.Now.AddMonths(-5),
                PackageId = 1
            };

            moqVersion = new PackageVersion
            {
                Id = 2,
                Major = 2,
                Minor = 2,
                Build = 2,
                Revision = -1,
                PublishedDate = DateTime.Now.AddMonths(-10),
                PackageId = 2
            };

            nUnitLatestVersion = new PackageVersion
            {
                Id = 3,
                Major = 2,
                Minor = 1,
                Build = -1,
                Revision = -1,
                PublishedDate = DateTime.Now,
                PackageId = 1
            };

            moqLatestVersion = new PackageVersion
            {
                Id = 4,
                Major = 3,
                Minor = 2,
                Build = 2,
                Revision = -1,
                PublishedDate = DateTime.Now,
                PackageId = 2
            };

            nUnitPackage = new Package
            {
                Id = 1,
                Name = "NUnit",
                Versions = new List<PackageVersion>
                {
                    nUnitVersion,
                },
                LastUpdateTime = null
            };

            moqPackage = new Package
            {
                Id = 2,
                Name = "Moq",
                Versions = new List<PackageVersion>
                {
                    moqVersion
                },
                LastUpdateTime = null
            };

            nUnitVersion.Package = nUnitPackage;

            moqVersion.Package = moqPackage;

            packageVersions = new List<PackageVersion>
            {
                nUnitVersion,
                moqVersion
            };

            packageLatestVersions = new List<PackageVersion>
            {
                nUnitLatestVersion,
                moqLatestVersion
            };

            packages = new List<Package>
            {
                nUnitPackage,
                moqPackage
            };

            projectPackageVersions = new List<ProjectPackageVersion>
            {
                new ProjectPackageVersion
                {
                    PackageVersionId = 1,
                    PackageVersion = nUnitVersion,
                    ProjectId = 1,
                    
                },
                new ProjectPackageVersion
                {
                    PackageVersionId = 2,
                    PackageVersion = moqVersion,
                    ProjectId = 1,
                }
            };

            project = new Project
            {
                Id = 1,
                Name = "TestProject",
                ProjectPackageVersions = projectPackageVersions
            };

            projects = new List<Project>
            {
                project
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
                .ReturnsAsync(packageLatestVersions);

            uowMock
                .Setup(uow => uow.ProjectsRepository)
                .Returns(projectsRepositoryMock.Object);

            uowMock
                .Setup(uow => uow.PackageVersionsRepository)
                .Returns(packageVersionsRepositoryMock.Object);

            versionAnalyzerServiceMock
                .Setup(versionAnalyzerService => versionAnalyzerService.Compare(nUnitLatestVersion, nUnitVersion))
                .Returns(nUnitReport);

            versionAnalyzerServiceMock
                .Setup(versionAnalyzerService => versionAnalyzerService.Compare(moqLatestVersion, moqVersion))
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
                versionAnalyzerService.Compare(nUnitLatestVersion, nUnitVersion));

            versionAnalyzerServiceMock.Verify(versionAnalyzerService =>
                versionAnalyzerService.Compare(moqLatestVersion, moqVersion));

            versionAnalyzerServiceMock.Verify(versionAnalyzerService =>
                versionAnalyzerService.CalculateMaxReportLevelStatus(reports));

            Assert.IsInstanceOf<ProjectReportDTO>(result);
            Assert.NotNull(result);
        }
    }
}