using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IVersionService versionService;
        private readonly IRepositoryRepository repositoryRepository;
        private readonly IVersionRepository versionRepository;

        public RepositoryService(IVersionService versionService, IRepositoryRepository repositoryRepository, IVersionRepository versionRepository)
        {
            this.versionService = versionService ?? throw new ArgumentNullException(nameof(versionService));
            this.repositoryRepository = repositoryRepository ?? throw new ArgumentNullException(nameof(repositoryRepository));
            this.versionRepository = versionRepository ?? throw new ArgumentNullException(nameof(versionRepository));
        }

        public async Task<ICollection<RepositoryWithVersionReport>> GetUserAnalyzedRepositoriesAsync(int userId)
        {
            var repositories = await repositoryRepository.GetUserRepositoriesWithIncludesAsync(userId);

            var packageIds = GetAllPackagesIdsFromRepositories(repositories);
            var latestPackageVersions = await versionRepository.GetLatestPackageVersionsWithPackageNameAsync(packageIds);

            var repositoriesWithVersionReport = Analyze(repositories, latestPackageVersions);

            return repositoriesWithVersionReport;
        }

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
                            .Select(ppv => versionService
                                .Compare(latestPackageVersions
                                    .First(lpv => lpv.PackageId == ppv.PackageVersion.PackageId), ppv.PackageVersion)).ToList();

                        repositoriesWithVersionReport[i]
                            .Solutions[j]
                            .Projects[k].Report = versionService.CalculateMaxReportLevelStatus(reports);
                    }

                    repositoriesWithVersionReport[i]
                        .Solutions[j].Report = versionService.CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i]
                            .Solutions[j].Projects
                            .Select(p => p.Report)
                            .ToList());
                }

                repositoriesWithVersionReport[i].Report = versionService.CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i]
                    .Solutions
                    .Select(p => p.Report)
                    .ToList());
            }

            return repositoriesWithVersionReport;
        }
    }
}