using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IPackageVersionsRepository : IRepository<PackageVersion>
    {
        Task<Dictionary<int, PackageVersion>> GetLatestPackageVersionsAsync(ICollection<int> packageIds);

        Task<IReadOnlyCollection<PackageVersion>> GetLatestVersionsAsync(Expression<Func<PackageVersion, bool>> predicate);

        Task<IReadOnlyCollection<PackageVersion>> GetAllLatestVersionsAsync();
    }
}
