using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;

namespace NugetAnalyzer.BLL.Services
{
	public class RepositoryService : IRepositoryService
	{
		private readonly IVersionsAnalyzerService versionsService;
		private readonly IUnitOfWork uow;
		private IRepositoriesRepository repositoriesRepository;

		public RepositoryService(IVersionsAnalyzerService versionsService, IUnitOfWork uow)
		{
			this.versionsService = versionsService ?? throw new ArgumentNullException(nameof(versionsService));
			this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		private IRepositoriesRepository RepositoriesRepository =>
			repositoriesRepository ?? (repositoriesRepository = uow.RepositoriesRepository);

		public async Task<ICollection<RepositoryWithVersionReport>> GetAnalyzedRepositoriesAsync(Expression<Func<Repository, bool>> expression)
		{
			IReadOnlyCollection<Repository> repositories = await RepositoriesRepository.GetRepositoriesWithIncludesAsync(expression);

			HashSet<int> packageIds = GetAllPackagesIdsFromRepositories(repositories);
			IReadOnlyCollection<PackageVersion> latestPackageVersions
				= await uow.PackageVersionsRepository.GetLatestPackageVersionsAsync(packageIds);

			ICollection<RepositoryWithVersionReport> repositoriesWithVersionReport = Analyze(repositories, latestPackageVersions);

			return repositoriesWithVersionReport;
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

		// TODO: give me an advice on this method, please =)
		private ICollection<RepositoryWithVersionReport> Analyze(
			IReadOnlyCollection<Repository> repositories,
			IReadOnlyCollection<PackageVersion> latestPackageVersions)
		{
			List<RepositoryWithVersionReport> repositoriesWithVersionReport
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
								.Compare(latestPackageVersions
									.First(packageVersion => packageVersion.PackageId == projectPackageVersion.PackageVersion.PackageId),
									projectPackageVersion.PackageVersion))
							.ToList();

						repositoriesWithVersionReport[i].Solutions[j].Projects[k].Report = versionsService.CalculateMaxReportLevelStatus(reports);
					}

					repositoriesWithVersionReport[i].Solutions[j].Report = versionsService
						.CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i]
							.Solutions[j].Projects
							.Select(projectWithVersionReport => projectWithVersionReport.Report)
							.ToList());
				}

				repositoriesWithVersionReport[i].Report = versionsService
					.CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i]
						.Solutions
						.Select(solutionWithVersionReport => solutionWithVersionReport.Report)
						.ToList());
			}

			return repositoriesWithVersionReport;
		}
	}
}