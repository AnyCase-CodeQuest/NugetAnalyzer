using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Repositories
{
    public class UsersRepository : Repository<User>, IUsersRepository
    {
        public UsersRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public Task<string> GetGitHubTokenByGitHubId(int gitHubId)
        {
            return DbSet
                .Where(p => p.GitHubId == gitHubId)
                .Select(p => p.GitHubToken)
                .FirstOrDefaultAsync();
        }
    }
}
