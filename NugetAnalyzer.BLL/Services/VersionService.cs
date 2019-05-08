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
        private readonly IVersionRepository versionRepository;
        private readonly IDateTimeProvider dateTimeProvider;

        public VersionService(IUnitOfWork uow, IDateTimeProvider dateTimeProvider)
        {
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
            this.dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            versionRepository = uow.VersionRepository;
        }

        public async Task UpdateLatestVersionOfNewPackagesAsync(IEnumerable<PackageVersion> versions)
        {
            var latestVersions = await versionRepository
                .GetLatestVersionsAsync(p => versions
                                                        .Select(x => x.PackageId)
                                                        .Contains(p.PackageId));

            await AddOrUpdateLatestVersionsAsync(versions, latestVersions);
        }

        public async Task UpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions)
        {
            var latestVersions = await versionRepository.GetAllLatestVersionsAsync();

            await AddOrUpdateLatestVersionsAsync(versions, latestVersions);
        }

        private async Task AddOrUpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions, IReadOnlyCollection<PackageVersion> latestVersions)
        {
            foreach (var packageVersion in versions)
            {
                var latestVersion = latestVersions.First(p => p.PackageId == packageVersion.PackageId);

                versionRepository.Attach(latestVersion);

                latestVersion
                        .Package
                        .LastUpdateTime = dateTimeProvider.CurrentUtcDateTime;

                if (latestVersion.GetVersion() == packageVersion.GetVersion())
                {
                    latestVersion.PublishedDate = packageVersion.PublishedDate;
                }
                else
                {
                    versionRepository.Add(packageVersion);
                }
            }

            await uow.SaveChangesAsync();
        }
    }
}