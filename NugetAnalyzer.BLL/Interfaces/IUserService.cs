using System.Threading.Tasks;
using NugetAnalyzer.BLL.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(Profile profile, string gitHubToken);

        Task<Profile> GetProfileByUserNameAsync(string userName);

        Task<Profile> GetProfileByGitHubIdAsync(int gitHubId);

        Task UpdateGitHubTokenAsync(int gitHubId, string gitHubToken);

        Task<string> GetGitHubTokenByGitHubIdAsync(int gitHubId);
    }
}