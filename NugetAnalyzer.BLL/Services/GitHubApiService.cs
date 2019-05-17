using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.Dtos.Models.Repositories;
using Octokit;
using Octokit.Internal;

namespace NugetAnalyzer.BLL.Services
{
    public class GitHubApiService : IGitHubApiService
    {
        private readonly string applicationName;

        private GitHubClient gitHubClient;

        public GitHubApiService(string applicationName)
        {
            this.applicationName = applicationName;
        }

        public async Task<IReadOnlyCollection<RepositoryChoice>> GetUserRepositoriesAsync(string userToken)
        {
            var repositories = await GetGitHubClientInstance(userToken)
                .Repository
                .GetAllForCurrent(new RepositoryRequest { Type = RepositoryType.Owner });
            return repositories
                .Select(RepositoryConverter.OctokitRepositoryToRepositoryChoice)
                .ToList();
        }

        public async Task<IReadOnlyCollection<Branch>> GetUserRepositoryBranchesAsync(string userToken, long repositoryId)
        {
            return await LoadRepositoryBranchesFromApiAsync(userToken, repositoryId);
        }

        //public async Task<IReadOnlyCollection<Branch>[]> GetAllUserRepositoriesBranchesAsync(string userToken, ICollection<long> repositoriesIds)
        //{
        //    var branchesTasks = repositoriesIds
        //        .Select(repositoryId => LoadRepositoryBranchesFromApiAsync(userToken, repositoryId))
        //        .ToList();

        //    return await Task.WhenAll(branchesTasks);
        //}

        private Task<IReadOnlyList<Branch>> LoadRepositoryBranchesFromApiAsync(string userToken, long repositoryId)
        {
            return GetGitHubClientInstance(userToken)
                .Repository
                .Branch
                .GetAll(repositoryId);
        }

        private GitHubClient GetGitHubClientInstance(string userToken)
        {
            return gitHubClient ?? (gitHubClient = new GitHubClient(
                       new ProductHeaderValue(applicationName),
                       new InMemoryCredentialStore(new Credentials(userToken))));
        }
    }
}
