using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IVersionService
    {
        Task UpdateLatestVersionOfNewPackagesAsync(IEnumerable<PackageVersion> versions);
        Task UpdateLatestVersionOfPackagesAsync(IEnumerable<PackageVersion> versions);
    }
}