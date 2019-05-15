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
        private IRepository<Package> packageRepository;

        public PackageService(IUnitOfWork uow)
        {
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
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

        public Task<IReadOnlyCollection<Package>> GetAllAsync()
        {
            return PackageRepository.GetAllAsync();
        }

        public Task<IReadOnlyCollection<Package>> GetNewlyAddedPackagesAsync()
        {
            return PackageRepository.GetAsync(package => package.LastUpdateTime == null);
        }
    }
}