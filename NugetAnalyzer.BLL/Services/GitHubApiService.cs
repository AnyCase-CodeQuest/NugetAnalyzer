using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using Octokit;
using Octokit.Internal;

namespace NugetAnalyzer.BLL.Services
{
    public class GitHubApiService : IGitHubApiService
    {
        public async Task<IReadOnlyCollection<Repository>> GetUserRepositoriesAsync(string token)
        {
            var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"),
                new InMemoryCredentialStore(new Credentials(token)));
            return await github.Repository.GetAllForCurrent();
        }
    }
}
