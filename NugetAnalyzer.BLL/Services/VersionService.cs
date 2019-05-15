using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Helpers;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class VersionService : IVersionService
    {
        private readonly IUnitOfWork uow;
        private readonly IDateTimeProvider dateTimeProvider;
        private IRepository<Package> packageRepository;
        private IVersionRepository versionRepository;

        public VersionService(IUnitOfWork uow, IDateTimeProvider dateTimeProvider)
        {
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        private IRepository<Package> PackageRepository
        {
            get
            {
                if (packageRepository == null)
                {
                    packageRepository = uow.GetRepository<Package>();
                }

                return packageRepository;
            }
        }

        private IVersionRepository VersionRepository
        {
            get
            {
                if (versionRepository == null)
                {
                    versionRepository = uow.VersionRepository;
                }

                return versionRepository;
            }
        }


        public async Task UpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions)
        {
            var latestVersions = await VersionRepository
                .GetLatestVersionsAsync(latestPackageVersion => versions
                                                        .Select(packageVersion => packageVersion.PackageId)
                                                        .Contains(latestPackageVersion.PackageId));

            await AddOrUpdateLatestVersionsAsync(versions, latestVersions);
        }

        public async Task UpdateAllLatestVersionsAsync(IEnumerable<PackageVersion> versions)
        {
            var latestVersions = await VersionRepository.GetAllLatestVersionsAsync();

            await AddOrUpdateLatestVersionsAsync(versions, latestVersions);
        }

        private async Task AddOrUpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions, IReadOnlyCollection<PackageVersion> latestVersions)
        {
            foreach (var packageVersion in versions)
            {
                var latestVersion = latestVersions.First(latestPackageVersion => latestPackageVersion.PackageId == packageVersion.PackageId);

                var package = latestVersion.Package;
                package.LastUpdateTime = dateTimeProvider.CurrentUtcDateTime;
                PackageRepository.Update(package);

                if (latestVersion.GetVersion() == packageVersion.GetVersion())
                {
                    latestVersion.PublishedDate = packageVersion.PublishedDate;
                    VersionRepository.Update(latestVersion);
                }
                else
                {
                    VersionRepository.Add(packageVersion);
                }
            }

            await uow.SaveChangesAsync();
        }
    }
}