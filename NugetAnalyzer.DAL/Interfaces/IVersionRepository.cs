using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IVersionRepository : IRepository<PackageVersion>
    {
        Task<IReadOnlyCollection<PackageVersion>> GetLatestVersionAsync(Expression<Func<PackageVersion, bool>> predicate);
        Task<IReadOnlyCollection<PackageVersion>> GetAllLatestVersionAsync();
    }
}