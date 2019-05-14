using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Models.Repositories;
using Octokit;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IGitHubApiService
    {
        Task<IReadOnlyCollection<RepositoryChoice>> GetUserRepositoriesAsync(string userToken);

        Task<IReadOnlyCollection<Branch>> GetUserRepositoryBranchesAsync(string userToken, long repositoryId);

        //Task<IReadOnlyCollection<Branch>[]> GetAllUserRepositoriesBranchesAsync(string userToken, ICollection<long> repositoriesIds);
    }
}
