using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IPackagesRepository : IRepository<Package>
    {
        Task<IReadOnlyCollection<Package>> GetIncludePackageVersionWithNotSetPublishedDateAsync();
    }
}