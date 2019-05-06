using System.Threading.Tasks;
using NugetAnalyzer.BLL.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(Profile profile, string gitHubToken);

        Profile GetProfileByUserName(string userName);
        Profile GetProfileByGitHubId(int gitHubId);

        Task UpdateGitHubToken(int gitHubId, string gitHubToken);
    }
}