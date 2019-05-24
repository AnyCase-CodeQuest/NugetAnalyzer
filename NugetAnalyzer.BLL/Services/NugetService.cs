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
        private readonly IPackageVersionService packageVersionService;
        private readonly IPackageService packageService;

        public NugetService(INugetApiService nugetApiService, IPackageVersionService packageVersionService, IPackageService packageService)
        {
            this.nugetApiService = nugetApiService ?? throw new ArgumentNullException(nameof(nugetApiService));
            this.packageVersionService = packageVersionService ?? throw new ArgumentNullException(nameof(packageVersionService));
            this.packageService = packageService ?? throw new ArgumentNullException(nameof(packageService));
        }

        public async Task RefreshLatestPackageVersionOfAllPackagesAsync()
        {
            IReadOnlyCollection<Package> newPackages = await packageService.GetAllAsync();

            Package[] packages = await GetPackageVersionsOfPackagesAsync(newPackages);

            await packageVersionService.UpdateAllLatestVersionsAsync(packages.Where(package => package != null).SelectMany(package => package.Versions));
        }

        public async Task RefreshNewlyAddedPackageVersionsAsync()
        {
            IReadOnlyCollection<Package> newPackages = await packageService.GetPackagesOfNewlyAddedPackageVersionsAsync();

            Package[] packages = await GetPackageVersionsOfPackagesAsync(newPackages);

            await packageVersionService.UpdateLatestVersionsAsync(packages.Where(package => package != null).SelectMany(package => package.Versions));
        }

        private async Task<Package[]> GetPackageVersionsOfPackagesAsync(IReadOnlyCollection<Package> packages)
        {
            IEnumerable<Task<Package>> packageVersionTasks = packages
                .Select(GetAllPackageVersionsOfPackageAsync);

            return await Task.WhenAll(packageVersionTasks);
        }

        private async Task<Package> GetAllPackageVersionsOfPackageAsync(Package package)
        {
            try
            {
                PackageVersion version = await nugetApiService.GetLatestPackageVersionAsync(package.Name);
                version.PackageId = package.Id;

                if (!package.Versions.Select(p => p.GetVersion()).Any())
                {
                    package.Versions.Add(version);
                }

                foreach (PackageVersion packageVersion in package.Versions)
                {
                    packageVersion.PublishedDate = await nugetApiService
                    .GetPackagePublishedDateByVersionAsync(package.Name, packageVersion.GetVersion().ToString());
                }
            }
            catch
            {
                return null;
            }
           
            return package;
        }
    }
}