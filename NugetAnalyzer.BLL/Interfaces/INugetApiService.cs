using System;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface INugetApiService
    {
        Task<PackageVersion> GetLatestPackageVersionAsync(string packageName);

        Task<DateTime?> GetPackagePublishedDateByVersionAsync(string packageName, string version);
    }
}