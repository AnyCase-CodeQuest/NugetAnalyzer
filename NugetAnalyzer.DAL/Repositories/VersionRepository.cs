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
    public class VersionRepository : Repository<PackageVersion>, IVersionRepository
    {
        public VersionRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<PackageVersion>> GetAllLatestVersionsAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Include(p => p.Package)
                .GroupByVersionAsync();
        }

        public async Task<IReadOnlyCollection<PackageVersion>> GetLatestVersionsAsync(Expression<Func<PackageVersion, bool>> predicate)
        {
            return await DbSet
                .AsNoTracking()
                .Include(p => p.Package)
                .Where(predicate)
                .GroupByVersionAsync();
        }
    }
}