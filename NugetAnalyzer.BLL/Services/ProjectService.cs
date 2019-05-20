using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork uow;
        private readonly IVersionsAnalyzerService versionsAnalyzerService;
        private IProjectsRepository projectRepository;
        private IPackageVersionsRepository packageVersionsRepository;

        public ProjectService(IUnitOfWork uow, IVersionsAnalyzerService versionsAnalyzerService)
        {
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
            this.versionsAnalyzerService = versionsAnalyzerService ?? throw new ArgumentNullException(nameof(versionsAnalyzerService));
        }

        private IProjectsRepository ProjectRepository =>
            projectRepository ?? (projectRepository = uow.ProjectsRepository);

        private IPackageVersionsRepository PackageVersionsRepository =>
            packageVersionsRepository ?? (packageVersionsRepository = uow.PackageVersionsRepository);

        public async Task<ProjectReportDTO> GetProjectReport(int projectId)
        {
            IReadOnlyCollection<Project> projects = await ProjectRepository.GetCollectionIncludePackage(proj => proj.Id == projectId);
            Project project = projects.FirstOrDefault();

            if (project == null)
            {
                return null;
            }

            IEnumerable<PackageVersion> currentPackageVersions = project
                                                                    .ProjectPackageVersions
                                                                    .Select(projectPackageVersion => projectPackageVersion.PackageVersion);

            IReadOnlyCollection<PackageVersion> latestPackageVersions = await PackageVersionsRepository
                .GetLatestVersionsAsync(x => currentPackageVersions
                    .Select(p => p.PackageId)
                    .Contains(x.PackageId));

            List<PackageReportDTO> packageReports = new List<PackageReportDTO>();

            foreach (PackageVersion packageVersion in currentPackageVersions)
            {
                PackageVersion latestVersion = latestPackageVersions.FirstOrDefault(p => p.PackageId == packageVersion.PackageId);
                PackageReportDTO packageVersionReport = new PackageReportDTO
                {
                    PackageId = packageVersion.PackageId,
                    LastUpdateTime = packageVersion.Package.LastUpdateTime,
                    PackageName = packageVersion.Package.Name,
                    Current = PackageVersionConverter.PackageVersionToPackageVersionDto(packageVersion),
                    Latest = PackageVersionConverter.PackageVersionToPackageVersionDto(latestVersion),
                    Report = versionsAnalyzerService.Compare(latestVersion, packageVersion)
                };
                packageReports.Add(packageVersionReport);
            }

            ProjectReportDTO projectReport = new ProjectReportDTO
            {
                Name = project.Name,
                Id = project.Id,
                Packages = packageReports,
                Report = versionsAnalyzerService
                    .CalculateMaxReportLevelStatus(packageReports.Select(p => p.Report).ToList())
            };

            return projectReport;
        }
    }
}