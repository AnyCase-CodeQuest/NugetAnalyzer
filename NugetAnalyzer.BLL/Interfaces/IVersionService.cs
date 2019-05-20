using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IVersionService
    {
        Task UpdateLatestVersionsAsync(IEnumerable<PackageVersion> versions);

        Task UpdateAllLatestVersionsAsync(IEnumerable<PackageVersion> versions);
    }
}