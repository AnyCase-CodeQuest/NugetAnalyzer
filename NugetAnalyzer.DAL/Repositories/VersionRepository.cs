using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<IReadOnlyCollection<PackageVersion>> GetAllLatestVersionAsync()
        {
            return await dbSet
                .Include(p => p.Package)
                .GroupBy(p => p.PackageId)
                .Select(grp =>
                    grp
                        .OrderByDescending(p => p.Major)
                        .ThenByDescending(p => p.Minor)
                        .ThenByDescending(p => p.Build)
                        .ThenByDescending(p => p.Revision)
                        .First())
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<PackageVersion>> GetLatestVersionAsync(Expression<Func<PackageVersion, bool>> predicate)
        {
            return await dbSet
                .Include(p => p.Package)
                .Where(predicate)
                .GroupBy(p => p.PackageId)
                .Select(grp =>
                    grp
                        .OrderByDescending(p => p.Major)
                        .ThenByDescending(p => p.Minor)
                        .ThenByDescending(p => p.Build)
                        .ThenByDescending(p => p.Revision)
                        .First())
                .ToListAsync();
        }
    }
}