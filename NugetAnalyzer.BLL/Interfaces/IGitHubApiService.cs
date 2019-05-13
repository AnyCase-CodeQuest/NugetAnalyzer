using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IGitHubApiService
    {
        Task<IReadOnlyCollection<Repository>> GetUserRepositoriesAsync(string token);
    }
}
