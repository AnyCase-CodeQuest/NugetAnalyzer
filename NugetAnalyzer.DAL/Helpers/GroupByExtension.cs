using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Helpers
{
    public static class GroupByExtension
    {
        public static async Task<IReadOnlyCollection<PackageVersion>> GroupByVersionAsync(
            this IQueryable<PackageVersion> source)
        {
            return await source
                .GroupBy(p => new { p.PackageId, p.Major, p.Minor, p.Build, p.Revision })
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