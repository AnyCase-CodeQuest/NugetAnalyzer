using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IPackageService
    {
        Task<IReadOnlyCollection<Package>> GetAllAsync();

        Task<IReadOnlyCollection<Package>> GetPackagesOfNewlyAddedPackageVersionsAsync();
    }
}