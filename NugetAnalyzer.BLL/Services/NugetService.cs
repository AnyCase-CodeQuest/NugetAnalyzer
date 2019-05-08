using System;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Helpers;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class NugetService : INugetService
    {
        private readonly INugetApiService nugetApiService;
        private readonly IVersionService versionService;
        private readonly IRepository<Package> packageRepository;

        public NugetService(INugetApiService nugetApiService, IVersionService versionService, IUnitOfWork uow)
        {
            this.nugetApiService = nugetApiService ?? throw new ArgumentNullException(nameof(nugetApiService));
            this.versionService = versionService ?? throw new ArgumentNullException(nameof(versionService));
            if (uow == null)
            {
                throw new ArgumentNullException(nameof(uow));
            }
            packageRepository = uow.GetRepository<Package>();
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
            var newPackages = await packageRepository.GetAllAsync();
            var packageVersionTasks = newPackages
                                        .Select(GetLatestVersionOfPackageAsync)
                                        .ToArray();

            var versions = await Task.WhenAll(packageVersionTasks);

            await versionService.UpdateLatestVersionOfPackagesAsync(versions);
        }

        public async Task RefreshLatestVersionOfNewPackagesAsync()
        {
            var newPackages = await packageRepository.GetAsync(p => p.LastUpdateTime == null);
            var packageVersionTasks = newPackages
                                        .Select(GetLatestVersionOfPackageAsync)
                                        .ToArray();

            var versions = await Task.WhenAll(packageVersionTasks);

            await versionService.UpdateLatestVersionOfNewPackagesAsync(versions);
        }
    }
}