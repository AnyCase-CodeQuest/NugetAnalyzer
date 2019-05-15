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

        public async Task<IReadOnlyCollection<PackageVersion>> GetLatestPackageVersionsAsync(ICollection<int> packageIds)
        {
            return await DbSet
                .Where(pv => packageIds.Contains(pv.PackageId))
                .GroupBy(p => p.PackageId)
                .Select(grp => grp
                    .OrderByDescending(p => p.Major)
                    .ThenByDescending(p => p.Minor)
                    .ThenByDescending(p => p.Build)
                    .ThenByDescending(p => p.Revision)
                    .First())
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
