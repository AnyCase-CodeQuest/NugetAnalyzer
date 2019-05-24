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
    public class PackagesRepository : Repository<Package>, IPackagesRepository
    {
        public PackagesRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Package>> GetIncludePackageVersionWithNotSetPublishedDateAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Include(package => package.Versions)
                .Where(package => package.Versions.Select(packageVersion => packageVersion.PublishedDate == null).Any())
                .ToListAsync();
        }
    }
}