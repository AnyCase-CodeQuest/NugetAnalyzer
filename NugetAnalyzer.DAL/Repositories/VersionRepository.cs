using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Repositories
{
    public class VersionRepository : Repository<PackageVersion>, IVersionRepository
    {
        public VersionRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<PackageVersion>> GetLatestPackageVersionsWithPackageNameAsync(ICollection<int> packageIds)
        {
            return await dbSet
                .Where(pv => packageIds.Contains(pv.Package.Id))
                .GroupBy(pv => pv.Package.Name)
                .Select(grp => grp
                    .OrderByDescending(pv => pv.Major)
                    .ThenByDescending(pv => pv.Minor)
                    .ThenByDescending(pv => pv.Build)
                    .ThenByDescending(pv => pv.Revision)
                    .First())
                .Include(pv => pv.Package.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
