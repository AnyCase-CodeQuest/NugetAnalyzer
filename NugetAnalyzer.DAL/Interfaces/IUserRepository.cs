using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        string GetGitHubTokenByGitHubId(int gitHubId);
    }
}
