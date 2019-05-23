using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Enums;
using NugetAnalyzer.DTOs.Models.Reports;
using NugetAnalyzer.DTOs.Models.Repositories;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IVersionsAnalyzerService versionsService;
        private readonly IGitService gitService;
        private readonly IRepositoryAnalyzerService repositoryAnalyzerService;
        private readonly IRepositorySaverService repositorySaverService;
        private readonly INugetService nugetService;
        private readonly IDirectoryService directoryService;

        private readonly ILogger<RepositoryService> logger;

        private readonly IUnitOfWork uow;
        private IRepositoriesRepository repositoriesRepository;

        public RepositoryService(IVersionsAnalyzerService versionsService,
            IGitService gitService,
            IRepositoryAnalyzerService repositoryAnalyzerService,
            IRepositorySaverService repositorySaverService,
            INugetService nugetService,
            IDirectoryService directoryService,
            ILogger<RepositoryService> logger,
            IUnitOfWork uow)
        {
            this.versionsService = versionsService ?? throw new ArgumentNullException(nameof(versionsService));
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
            this.repositoryAnalyzerService = repositoryAnalyzerService ?? throw new ArgumentNullException(nameof(repositoryAnalyzerService));
            this.repositorySaverService = repositorySaverService ?? throw new ArgumentNullException(nameof(repositorySaverService));
            this.nugetService = nugetService ?? throw new ArgumentNullException(nameof(nugetService));
            this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        private IRepositoriesRepository RepositoriesRepository =>
            repositoriesRepository ?? (repositoriesRepository = uow.RepositoriesRepository);

        public async Task<ICollection<RepositoryReport>> GetAnalyzedRepositoriesAsync(Expression<Func<Repository, bool>> expression)
        {
            IReadOnlyCollection<Repository> repositories = await RepositoriesRepository.GetRepositoriesWithIncludesAsync(expression);

            HashSet<int> packageIds = GetAllPackagesIdsFromRepositories(repositories);
            Dictionary<int, PackageVersion> latestPackageVersions
                = await uow.PackageVersionsRepository.GetLatestPackageVersionsAsync(packageIds);

            ICollection<RepositoryReport> repositoriesVersionReport = Analyze(repositories, latestPackageVersions);

            return repositoriesVersionReport;
        }

        public async Task<IReadOnlyCollection<string>> GetRepositoriesNamesAsync(Expression<Func<Repository, bool>> expression)
        {
            return await RepositoriesRepository.GetRepositoriesNamesAsync(expression);
        }

        public async Task<AddRepositoriesResponseDTO> AddRepositoriesAsync(Dictionary<string, string> repositories, string userToken, int userId)
        {
            var tasks = new List<Task<string>>();
            
            foreach (var repository in repositories)
            {
                tasks.Add(AddAsync(repository, userToken, userId));
            }

            var addedRepositoriesNames = (await Task.WhenAll(tasks))
                .Where(name => name != null)
                .ToList();

            var addedRepositoriesResponse = new AddRepositoriesResponseDTO
            {
                ResponseType = repositories.Count == addedRepositoriesNames.Count
                    ? ResponseType.Info
                    : ResponseType.Error,
                RepositoriesNames = addedRepositoriesNames
            };

            return addedRepositoriesResponse;
        }

        private async Task<string> AddAsync(KeyValuePair<string, string> repository, string userToken, int userId)
        {
            string clonePath = directoryService.GenerateClonePath();
            var repositoryName = directoryService.GetName(repository.Key);
            var repositoryPath = $"{clonePath}/{repositoryName}";
            try
            {
                gitService.CloneBranch(repository.Key, repositoryPath, userToken, repository.Value);
                var parsedRepository = await repositoryAnalyzerService.GetParsedRepositoryAsync(repositoryPath);
                await repositorySaverService.SaveAsync(parsedRepository, userId);

                await nugetService.RefreshLatestVersionOfNewlyAddedPackagesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return null;
            }
            finally
            {
                directoryService.Delete(clonePath); // TODO: async ?
            }

            return repositoryName;
        }

        private HashSet<int> GetAllPackagesIdsFromRepositories(IEnumerable<Repository> repositories)
        {
            IEnumerable<int> packageIds = repositories
                .SelectMany(repository => repository.Solutions)
                .SelectMany(solution => solution.Projects)
                .SelectMany(project => project.ProjectPackageVersions)
                .Select(projectPackageVersion => projectPackageVersion.PackageVersion.PackageId)
                .Distinct();

            return new HashSet<int>(packageIds);
        }

        private ICollection<RepositoryReport> Analyze(
            IReadOnlyCollection<Repository> repositories,
            Dictionary<int, PackageVersion> latestPackageVersions)
        {
            List<RepositoryReport> repositoriesVersionReports
                = repositories.Select(RepositoryConverter.RepositoryToRepositoryVersionReport).ToList();

            for (int i = 0; i < repositories.Count; i++)
            {
                List<Solution> solutions = repositories
                    .ElementAt(i)
                    .Solutions
                    .ToList();

                for (int j = 0; j < solutions.Count; j++)
                {
                    List<Project> projects = solutions[j]
                        .Projects
                        .ToList();

                    for (int k = 0; k < projects.Count; k++)
                    {
                        List<PackageVersionComparisonReport> reports = projects[k]
                            .ProjectPackageVersions
                            .Select(projectPackageVersion => versionsService
                                .Compare(latestPackageVersions[projectPackageVersion.PackageVersion.PackageId],
                        projectPackageVersion.PackageVersion))
                            .ToList();

                        repositoriesVersionReports[i].Solutions[j].Projects[k].Report = versionsService.CalculateMaxReportLevelStatus(reports);
                    }

                    repositoriesVersionReports[i].Solutions[j].Report = versionsService
                        .CalculateMaxReportLevelStatus(repositoriesVersionReports[i]
                            .Solutions[j].Projects
                            .Select(projectVersionReport => projectVersionReport.Report)
                            .ToList());
                }

                repositoriesVersionReports[i].Report = versionsService
                    .CalculateMaxReportLevelStatus(repositoriesVersionReports[i]
                        .Solutions
                        .Select(solutionVersionReport => solutionVersionReport.Report)
                        .ToList());
            }

            return repositoriesVersionReports;
        }
    }
}