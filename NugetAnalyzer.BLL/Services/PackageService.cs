using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork uow;
        private IPackagesRepository packagesRepository;

        public PackageService(IUnitOfWork uow)
        {
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        private IPackagesRepository PackagesRepository => 
            packagesRepository ?? (packagesRepository = uow.PackagesRepository);

        public Task<IReadOnlyCollection<Package>> GetAllAsync()
        {
            return PackagesRepository.GetAllAsync();
        }

        public Task<IReadOnlyCollection<Package>> GetPackagesOfNewlyAddedPackageVersionsAsync()
        {
            return PackagesRepository.GetIncludePackageVersionWithNotSetPublishedDateAsync();
        }
    }
}