using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Helpers;
using NugetAnalyzer.BLL.Interfaces;
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
            this.uow = uow;
            this.dateTimeProvider = dateTimeProvider;
            versionRepository = uow.GetRepository<IVersionRepository>();
        }

        public async Task UpdateLatestVersionOfNewPackagesAsync(IEnumerable<PackageVersion> versions)
        {
            var latestVersions = await versionRepository
                .GetLatestVersionAsync(p => versions
                                                        .Select(x => x.PackageId)
                                                        .Contains(p.PackageId));
            
            foreach (var packageVersion in versions)
            {
                var latestVersion = latestVersions.First(p => p.PackageId == packageVersion.PackageId);

                latestVersion
                    .Package
                    .LastUpdateTime = dateTimeProvider.CurrentDateAndTime;

                if (latestVersion.GetVersion() == packageVersion.GetVersion())
                {
                    latestVersion.PublishedDate = packageVersion.PublishedDate;
                    versionRepository.Update(latestVersion);
                }
                else
                {
                    versionRepository.Add(packageVersion);
                }
            }

            await uow.SaveChangesAsync();
        }

        public async Task UpdateLatestVersionOfPackagesAsync(IEnumerable<PackageVersion> versions)
        {
            var latestVersions = await versionRepository.GetAllLatestVersionAsync();

            foreach (var packageVersion in versions)
            {
                var latestVersion = latestVersions.First(p => p.PackageId == packageVersion.PackageId);

                latestVersion
                    .Package
                    .LastUpdateTime = dateTimeProvider.CurrentDateAndTime;

                if (latestVersion.GetVersion() == packageVersion.GetVersion())
                {
                    versionRepository.Update(latestVersion);
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