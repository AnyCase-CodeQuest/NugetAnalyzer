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
    public class PackageVersionService : IPackageVersionService
    {
        private readonly IUnitOfWork uow;
        private readonly IDateTimeProvider dateTimeProvider;
        private IRepository<Package> packageRepository;
        private IPackageVersionsRepository packageVersionsRepository;

        public PackageVersionService(IUnitOfWork uow, IDateTimeProvider dateTimeProvider)
        {
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        private IRepository<Package> PackageRepository =>
            packageRepository ?? (packageRepository = uow.GetRepository<Package>());

        private IPackageVersionsRepository PackageVersionsRepository =>
            packageVersionsRepository ?? (packageVersionsRepository = uow.PackageVersionsRepository);

        public async Task UpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions)
        {
            IReadOnlyCollection<PackageVersion> latestVersions = await PackageVersionsRepository
                .GetLatestVersionsAsync(latestPackageVersion => versions
                                                        .Select(packageVersion => packageVersion.PackageId)
                                                        .Contains(latestPackageVersion.PackageId));

            await AddOrUpdatePackageVersionsAsync(versions, latestVersions);
        }

        public async Task UpdateAllLatestVersionsAsync(IEnumerable<PackageVersion> versions)
        {
            IReadOnlyCollection<PackageVersion> latestVersions = await PackageVersionsRepository.GetAllLatestVersionsAsync();

            await AddOrUpdatePackageVersionsAsync(versions, latestVersions);
        }

        private async Task AddOrUpdatePackageVersionsAsync(
            IEnumerable<PackageVersion> versions, IReadOnlyCollection<PackageVersion> latestVersions)
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
                else if (packageVersion.Id > 0)
                {
                    PackageVersionsRepository.Update(packageVersion);
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