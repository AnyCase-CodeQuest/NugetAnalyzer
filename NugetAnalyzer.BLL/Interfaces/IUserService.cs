using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(ProfileViewModel profile);

        Task<ProfileViewModel> GetProfileByUserNameAsync(string userName);

        Task<ProfileViewModel> GetProfileByGitHubIdAsync(int gitHubId);

        Task UpdateUserAsync(ProfileViewModel profile);
    }
}