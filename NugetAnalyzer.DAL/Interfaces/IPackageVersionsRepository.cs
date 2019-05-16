using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IPackageVersionsRepository
    {
        Task<IReadOnlyCollection<PackageVersion>> GetLatestPackageVersionsAsync(ICollection<int> packageIds);
    }
}
