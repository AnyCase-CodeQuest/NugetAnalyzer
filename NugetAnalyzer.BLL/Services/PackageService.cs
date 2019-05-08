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
        private readonly IRepository<Package> packageRepository;

        public PackageService(IUnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException(nameof(uow));
            }
            packageRepository = uow.GetRepository<Package>();
        }

        public Task<IReadOnlyCollection<Package>> GetAllAsync()
        {
            return packageRepository.GetAllAsync();
        }

        public Task<IReadOnlyCollection<Package>> GetNewPackagesAsync()
        {
            return packageRepository.GetAsync(p => p.LastUpdateTime == null);
        }
    }
}