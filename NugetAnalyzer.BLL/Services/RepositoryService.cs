using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.BLL.Services
{
	public class RepositoryService : IRepositoryService
	{
		private readonly IVersionsAnalyzerService versionsService;
        private readonly IGitService gitService;

		private readonly IUnitOfWork uow;
		private IRepositoriesRepository repositoriesRepository;

		public RepositoryService(IVersionsAnalyzerService versionsService, IGitService gitService, IUnitOfWork uow)
		{
			this.versionsService = versionsService ?? throw new ArgumentNullException(nameof(versionsService));
			this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
			this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		private IRepositoriesRepository RepositoriesRepository =>
			repositoriesRepository ?? (repositoriesRepository = uow.RepositoriesRepository);

		public async Task<ICollection<RepositoryVersionReport>> GetAnalyzedRepositoriesAsync(Expression<Func<Repository, bool>> expression)
		{
			IReadOnlyCollection<Repository> repositories = await RepositoriesRepository.GetRepositoriesWithIncludesAsync(expression);

			HashSet<int> packageIds = GetAllPackagesIdsFromRepositories(repositories);
            Dictionary<int, PackageVersion> latestPackageVersions
				= await uow.PackageVersionsRepository.GetLatestPackageVersionsAsync(packageIds);

			ICollection<RepositoryVersionReport> repositoriesVersionReport = Analyze(repositories, latestPackageVersions);

			return repositoriesVersionReport;
		}

        public async Task<IReadOnlyCollection<string>> GetRepositoriesNamesAsync(Expression<Func<Repository, bool>> expression)
        {
            return await RepositoriesRepository.GetRepositoriesNamesAsync(expression);
        }

        public async Task AddRepositoriesAsync(Dictionary<string, string> repositories, string userToken)
        {
            var tasks = new List<Task>();
            foreach (var repository in repositories)
            {
                tasks.Add(Task.Run(() =>  // TODO: throws exceptions
                {
                    var generatedPath = GeneratePathForClone();
                    gitService.CloneBranch(repository.Key, generatedPath, userToken, repository.Value);
                    // Analyze data
                    // Save to db;
                    Directory.Delete(generatedPath, true);
                }));
            }

            await Task.WhenAll(tasks);
        }

        private string GeneratePathForClone()
        {
            throw new NotImplementedException();
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

		private ICollection<RepositoryVersionReport> Analyze(
			IReadOnlyCollection<Repository> repositories,
            Dictionary<int, PackageVersion> latestPackageVersions)
		{
			List<RepositoryVersionReport> repositoriesVersionReports
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