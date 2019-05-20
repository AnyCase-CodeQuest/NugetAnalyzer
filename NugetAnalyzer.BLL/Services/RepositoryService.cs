﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Reports;

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

        private readonly IUnitOfWork uow;
		private IRepositoriesRepository repositoriesRepository;

		public RepositoryService(IVersionsAnalyzerService versionsService,
            IGitService gitService,
            IRepositoryAnalyzerService repositoryAnalyzerService,
            IRepositorySaverService repositorySaverService,
            INugetService nugetService,
            IDirectoryService directoryService,
            IUnitOfWork uow)
		{
			this.versionsService = versionsService ?? throw new ArgumentNullException(nameof(versionsService));
			this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
			this.repositoryAnalyzerService = repositoryAnalyzerService ?? throw new ArgumentNullException(nameof(repositoryAnalyzerService));
			this.repositorySaverService = repositorySaverService ?? throw new ArgumentNullException(nameof(repositorySaverService));
			this.nugetService = nugetService ?? throw new ArgumentNullException(nameof(nugetService));
			this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
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

        public async Task AddRepositoriesAsync(Dictionary<string, string> repositories, string userToken, int userId)
        {
            var tasks = new List<Task>();
            foreach (var repository in repositories)
            {
                //tasks.Add(Task.Run(async () =>
                //{
                    string directoryPath = null;
                    try
                    {
                        directoryPath = directoryService.GeneratePath(directoryService.GetName(repository.Key));
                        gitService.CloneBranch(repository.Key, directoryPath, userToken, repository.Value);
                        var parsedRepository = repositoryAnalyzerService.GetParsedRepositoryAsync(directoryPath).Result;
                        repositorySaverService.SaveAsync(parsedRepository, userId).Wait();
                        nugetService.RefreshLatestVersionOfNewlyAddedPackagesAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        //TODO: logging
                    }
                    finally
                    {
                        directoryService.Delete(directoryPath); // TODO !!!
                    }
                //}));
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