using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Helpers;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class NugetService : INugetService
    {
        private readonly INugetApiService nugetApiService;
        private readonly IVersionService versionService;
        private readonly IPackageService packageService;

        public NugetService(INugetApiService nugetApiService, IVersionService versionService, IPackageService packageService)
        {
            this.nugetApiService = nugetApiService ?? throw new ArgumentNullException(nameof(nugetApiService));
            this.versionService = versionService ?? throw new ArgumentNullException(nameof(versionService));
            this.packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
        }

        public async Task RefreshLatestVersionOfAllPackagesAsync()
        {
            IReadOnlyCollection<Package> newPackages = await packageService.GetAllAsync();

            PackageVersion[] versions = await GetLatestVersionsOfPackagesAsync(newPackages);

            await versionService.UpdateAllLatestVersionsAsync(versions.Where(packageVersion => packageVersion != null));
        }

        public async Task RefreshLatestVersionOfNewlyAddedPackagesAsync()
        {
            IReadOnlyCollection<Package> newPackages = await packageService.GetNewlyAddedPackagesAsync();

            PackageVersion[] versions = await GetLatestVersionsOfPackagesAsync(newPackages);

            await versionService.UpdateLatestVersionsAsync(versions.Where(packageVersion => packageVersion != null));
        }


        private async Task<PackageVersion[]> GetLatestVersionsOfPackagesAsync(IReadOnlyCollection<Package> packages)
        {
            IEnumerable<Task<PackageVersion>> packageVersionTasks = packages
                .Select(GetLatestVersionOfPackageAsync);

            return await Task.WhenAll(packageVersionTasks);
        }

        private async Task<PackageVersion> GetLatestVersionOfPackageAsync(Package package)
        {
            PackageVersion version = null;
            try
            {
                version = await nugetApiService.GetLatestPackageVersionAsync(package.Name);

                version.PublishedDate = await nugetApiService
                    .GetPackagePublishedDateByVersionAsync(package.Name, version.GetVersion().ToString());
            }
            catch
            {
                return null;
            }
           
            version.PackageId = package.Id;
            return version;
        }
    }
}