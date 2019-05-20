using System.Linq;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Helpers
{
    public static class GroupByExtension
    {
        public static IQueryable<PackageVersion> GroupByPackagesAsync(
            this IQueryable<PackageVersion> source)
        {
            return source
                .GroupBy(packageVersion => packageVersion.PackageId)
                .Select(grouping =>
                    grouping
                        .OrderByDescending(packageVersion => packageVersion.Major)
                        .ThenByDescending(packageVersion => packageVersion.Minor)
                        .ThenByDescending(packageVersion => packageVersion.Build)
                        .ThenByDescending(packageVersion => packageVersion.Revision)
                        .First());
        }
    }
}