using System;
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

        private async Task<PackageVersion> GetLatestVersionOfPackageAsync(Package package)
        {
            var version = await nugetApiService.GetLatestVersionPackageAsync(package.Name);

            version.PublishedDate = await nugetApiService
                .GetPublishedDateByVersionAsync(package.Name, version.GetVersion().ToString());

            version.PackageId = package.Id;

            return version;
        }
       
        public async Task RefreshLatestVersionOfAllPackagesAsync()
        {
            var newPackages = await packageService.GetAllAsync();
            var packageVersionTasks = newPackages
                                        .Select(GetLatestVersionOfPackageAsync)
                                        .ToArray();

            var versions = await Task.WhenAll(packageVersionTasks);

            await versionService.UpdateLatestVersionsAsync(versions);
        }

        public async Task RefreshLatestVersionOfNewPackagesAsync()
        {
            var newPackages = await packageService.GetNewPackagesAsync();
            var packageVersionTasks = newPackages
                                        .Select(GetLatestVersionOfPackageAsync)
                                        .ToArray();

            var versions = await Task.WhenAll(packageVersionTasks);

            await versionService.UpdateLatestVersionOfNewPackagesAsync(versions);
        }
    }
}