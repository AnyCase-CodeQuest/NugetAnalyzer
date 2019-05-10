using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Repositories
{
    public class RepositoryRepository : Repository<Repository>, IRepositoryRepository
    {
        public RepositoryRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Repository>> GetUserRepositoriesWithIncludesAsync(int userId)
        {
            return await dbSet.Where(r => r.UserId == userId)
                .Include(r => r.Solutions)
                .ThenInclude(s => s.Projects)
                .ThenInclude(p => p.ProjectPackageVersions)
                .ThenInclude(ppv => ppv.PackageVersion)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
