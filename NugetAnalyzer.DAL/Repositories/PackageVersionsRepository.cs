using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Helpers;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Repositories
{
    public class PackageVersionsRepository : Repository<PackageVersion>, IPackageVersionsRepository
    {
        public PackageVersionsRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public async Task<Dictionary<int, PackageVersion>> GetLatestPackageVersionsAsync(ICollection<int> packageIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(packageVersion => packageIds.Contains(packageVersion.PackageId))
                .GroupByPackagesAsync()
                .ToDictionaryAsync(packageVersion => packageVersion.PackageId, packageVersion => packageVersion);
        }

        public async Task<IReadOnlyCollection<PackageVersion>> GetAllLatestVersionsAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Include(packageVersion => packageVersion.Package)
                .GroupByPackagesAsync()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<PackageVersion>> GetLatestVersionsAsync(Expression<Func<PackageVersion, bool>> predicate)
        {
            return await DbSet
                .AsNoTracking()
                .Include(packageVersion => packageVersion.Package)
                .Where(predicate)
                .GroupByPackagesAsync()
                .ToListAsync();
        }
    }
}
