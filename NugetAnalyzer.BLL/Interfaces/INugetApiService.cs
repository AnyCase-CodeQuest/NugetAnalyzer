using System;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface INugetApiService
    {
        Task<PackageVersion> GetLatestVersionPackageAsync(string packageName);
        Task<DateTime?> GetPublishedDateByVersionAsync(string packageName, string version);
    }
}