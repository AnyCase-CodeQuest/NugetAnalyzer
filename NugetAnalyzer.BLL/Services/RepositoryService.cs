using System;
using System.Collections.Generic;
using System.Linq;
using NugetAnalyzer.BLL.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IPackageVersionService packageVersionService;
        private readonly IRepositoryRepository repositoryRepository;

        public RepositoryService(IPackageVersionService packageVersionService, IRepositoryRepository repositoryRepository)
        {
            this.packageVersionService = packageVersionService ?? throw new ArgumentNullException(nameof(packageVersionService));
            this.repositoryRepository = repositoryRepository ?? throw new ArgumentNullException(nameof(repositoryRepository));
        }

        public ICollection<RepositoryWithVersionReport> GetUserAnalyzedRepositories(int userId)
        {
            var repositories = repositoryRepository.GetUserRepositoriesWithIncludes(userId).Result;
            var repositoriesWithVersionReport = Analyze(repositories);
            return repositoriesWithVersionReport;
        }

        private List<RepositoryWithVersionReport> Analyze(IReadOnlyCollection<Repository> repositories)
        {
            var repositoriesWithVersionReport = repositories.Select(RepositoryConverter.RepositoryToRepositoryWithVersionReport).ToList();

            for (var i = 0; i < repositories.Count; i++)
            {
                var solutions = repositories.ElementAt(i).Solutions.ToList();
                for (var j = 0; j < solutions.Count; j++)
                {
                    var projects = solutions.ElementAt(j).Projects.ToList();
                    for (var k = 0; k < projects.Count; k++)
                    {
                        var reports = projects.ElementAt(k).ProjectPackageVersions.Select(ppv => packageVersionService.Compare(null, ppv.PackageVersion));
                        repositoriesWithVersionReport[i].Solutions.ToList()[j].Projects.ToList()[k].Report = packageVersionService.CalculateMaxReportLevelStatus(reports.ToList());
                    }
                    repositoriesWithVersionReport[i].Solutions.ToList()[j].Report = packageVersionService
                        .CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i].Solutions.ToList()[j].Projects.Select(p => p.Report).ToList());
                }
                repositoriesWithVersionReport[i].Report = packageVersionService
                    .CalculateMaxReportLevelStatus(repositoriesWithVersionReport[i].Solutions.Select(p => p.Report).ToList());
            }

            return repositoriesWithVersionReport;
        }
    }
}