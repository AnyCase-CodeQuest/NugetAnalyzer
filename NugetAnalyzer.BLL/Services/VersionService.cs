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
        private IPackageVersionsRepository packageVersionsRepository;

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

        private IPackageVersionsRepository PackageVersionsRepository
        {
            get
            {
                if (packageVersionsRepository == null)
                {
                    packageVersionsRepository = uow.PackageVersionsRepository;
                }

                return packageVersionsRepository;
            }
        }


        public async Task UpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions)
        {
            IReadOnlyCollection<PackageVersion> latestVersions = await PackageVersionsRepository
                .GetLatestVersionsAsync(latestPackageVersion => versions
                                                        .Select(packageVersion => packageVersion.PackageId)
                                                        .Contains(latestPackageVersion.PackageId));

            await AddOrUpdateLatestVersionsAsync(versions, latestVersions);
        }

        public async Task UpdateAllLatestVersionsAsync(IEnumerable<PackageVersion> versions)
        {
            IReadOnlyCollection<PackageVersion> latestVersions = await PackageVersionsRepository.GetAllLatestVersionsAsync();

            await AddOrUpdateLatestVersionsAsync(versions, latestVersions);
        }

        private async Task AddOrUpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions, IReadOnlyCollection<PackageVersion> latestVersions)
        {
            foreach (PackageVersion packageVersion in versions)
            {
                PackageVersion latestVersion = latestVersions.First(latestPackageVersion => latestPackageVersion.PackageId == packageVersion.PackageId);

                Package package = latestVersion.Package;
                package.LastUpdateTime = dateTimeProvider.CurrentUtcDateTime;
                PackageRepository.Update(package);

                if (latestVersion.GetVersion() == packageVersion.GetVersion())
                {
                    latestVersion.PublishedDate = packageVersion.PublishedDate;
                    PackageVersionsRepository.Update(latestVersion);
                }
                else
                {
                    PackageVersionsRepository.Add(packageVersion);
                }
            }

            await uow.SaveChangesAsync();
        }
    }
}