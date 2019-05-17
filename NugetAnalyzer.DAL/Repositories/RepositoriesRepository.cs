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
    public class RepositoriesRepository : Repository<Repository>, IRepositoriesRepository
    {
        public RepositoriesRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Repository>> GetRepositoriesWithIncludesAsync(Expression<Func<Repository, bool>> expression)
        {
            return await DbSet
                .AsNoTracking()
                .Where(expression)
                .Include(r => r.Solutions)
                .ThenInclude(s => s.Projects)
                .ThenInclude(p => p.ProjectPackageVersions)
                .ThenInclude(ppv => ppv.PackageVersion)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<string>> GetRepositoriesNamesAsync(Expression<Func<Repository, bool>> expression)
        {
            return await DbSet
                .AsNoTracking()
                .Where(expression)
                .Select(repository => repository.Name)
                .ToListAsync();
        }
    }
}
