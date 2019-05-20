using System;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.Web.Infrastructure.Extensions
{
    public static class PackageVersionExtension
    {
        public static Version GetVersion(this PackageVersionDTO packageVersion)
        {
            if (packageVersion.Build < 0 && packageVersion.Revision < 0)
            {
                return new Version(packageVersion.Major, packageVersion.Minor);
            }

            return packageVersion.Revision < 0
                ? new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build)
                : new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
    }
}