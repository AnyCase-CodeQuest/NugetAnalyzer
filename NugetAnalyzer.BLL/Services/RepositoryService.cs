using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models.Repositories;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IVersionAnalyzerService versionService;
        private readonly IUnitOfWork uow;
        private IRepositoryRepository repositoryRepository;

        public RepositoryService(IVersionAnalyzerService versionService, IUnitOfWork uow)
        {
            this.versionService = versionService ?? throw new ArgumentNullException(nameof(versionService));
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<ICollection<RepositoryWithVersionReport>> GetAnalyzedRepositoriesAsync(Expression<Func<Repository, bool>> expression)
        {
            var repositories = await RepositoryRepository.GetRepositoriesWithIncludesAsync(expression);

            var packageIds = GetAllPackagesIdsFromRepositories(repositories);
            var latestPackageVersions = await uow.VersionRepository.GetLatestPackageVersionsAsync(packageIds);

            var repositoriesWithVersionReport = Analyze(repositories, latestPackageVersions);

            return repositoriesWithVersionReport;
        }

        private IRepositoryRepository RepositoryRepository =>
            repositoryRepository ?? (repositoryRepository = uow.RepositoryRepository);

        private HashSet<int> GetAllPackagesIdsFromRepositories(IEnumerable<Repository> repositories)
        {
            var packageIds = new HashSet<int>();
            foreach (var repository in repositories)
            {
                foreach (var solution in repository.Solutions)
                {
                    foreach (var project in solution.Projects)
                    {
                        foreach (var projectPackageVersion in project.ProjectPackageVersions)
                        {
                            packageIds.Add(projectPackageVersion.PackageVersion.PackageId);
                        }
                    }
                }
            }

            return packageIds;
        }

        // TODO: give me an advice on this method, please =)
        private ICollection<RepositoryWithVersionReport> Analyze(IReadOnlyCollection<Repository> repositories, IReadOnlyCollection<PackageVersion> latestPackageVersions)
        {
            var repositoriesWithVersionReport = repositories.Select(RepositoryConverter.RepositoryToRepositoryWithVersionReport).ToList();

            for (var i = 0; i < repositories.Count; i++)
            {
                var solutions = repositories
                    .ElementAt(i)
                    .Solutions
                    .ToList();

                for (var j = 0; j < solutions.Count; j++)
                {
                    var projects = solutions
                        .ElementAt(j)
                        .Projects
                        .ToList();

                    for (var k = 0; k < projects.Count; k++)
                    {
                        var reports = projects
                            .ElementAt(k)
                            .ProjectPackageVersions
                            .Select(projectPackageVersion => versionService
                                .Compare(latestPackageVersions
                                    .First(packageVersion => packageVersion.PackageId == projectPackageVersion.PackageVersion.PackageId), projectPackageVersion.PackageVersion))
                            .ToList();

                        repositoriesWithVersionReport[i]
                            .Solutions[j]
                            .Projects[k].Report = versionService.CalculateMaxReportLevelStatus(reports);
                    }

                    repositoriesWithVersionReport[i]
                        .Solutions[j].Report = versionService.CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i]
                            .Solutions[j].Projects
                            .Select(projectWithVersionReport => projectWithVersionReport.Report)
                            .ToList());
                }

                repositoriesWithVersionReport[i].Report = versionService.CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i]
                    .Solutions
                    .Select(solutionWithVersionReport => solutionWithVersionReport.Report)
                    .ToList());
            }

            return repositoriesWithVersionReport;
        }
    }
}