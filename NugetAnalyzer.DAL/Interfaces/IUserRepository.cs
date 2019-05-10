using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUsersRepository : IRepository<User>
    {
        Task<string> GetGitHubTokenByGitHubId(int gitHubId);
    }
}
