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
                .GroupBy(pv => new { pv.PackageId, pv.Major, pv.Minor, pv.Build, pv.Revision })
                .Select(grp =>
                    grp
                        .OrderByDescending(pv => pv.Major)
                        .ThenByDescending(pv => pv.Minor)
                        .ThenByDescending(pv => pv.Build)
                        .ThenByDescending(pv => pv.Revision)
                        .First())
                .ToListAsync();
        }
    }
}