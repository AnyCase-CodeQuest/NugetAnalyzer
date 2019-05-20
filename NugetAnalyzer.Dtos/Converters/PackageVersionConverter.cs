using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.DTOs.Converters
{
    public static class PackageVersionConverter
    {
        public static PackageVersionDTO PackageVersionToPackageVersionDto(PackageVersion packageVersion)
        {
            return packageVersion == null
                ? null
                : new PackageVersionDTO
                {
                    Id = packageVersion.Id,
                    Major = packageVersion.Major,
                    Minor = packageVersion.Minor,
                    Build = packageVersion.Build,
                    Revision = packageVersion.Revision,
                    PublishedDate = packageVersion.PublishedDate,
                    PackageId = packageVersion.PackageId
                };
        }
    }
}
