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
                .GroupBy(packageVersion => packageVersion.PackageId)
                .Select(grouping =>
                    grouping
                        .OrderByDescending(packageVersion => packageVersion.Major)
                        .ThenByDescending(packageVersion => packageVersion.Minor)
                        .ThenByDescending(packageVersion => packageVersion.Build)
                        .ThenByDescending(packageVersion => packageVersion.Revision)
                        .First())
                .ToListAsync();
        }
    }
}