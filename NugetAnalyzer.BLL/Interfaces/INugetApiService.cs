using System;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    /// <summary>
    /// Used to work with Nuget API
    /// </summary>
    public interface INugetApiService
    {
        /// <summary>
        /// Gets the latest package version by package name from Nuget Api asynchronous.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <returns>The latest package version.</returns>
        Task<PackageVersion> GetLatestPackageVersionAsync(string packageName);

        /// <summary>
        /// Gets the package published date by package version asynchronous.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="version">The package version.</param>
        /// <returns>The package published date.</returns>
        Task<DateTime?> GetPackagePublishedDateByVersionAsync(string packageName, string version);
    }
}