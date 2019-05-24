using System.Threading.Tasks;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface INugetService
    {
        Task RefreshLatestPackageVersionOfAllPackagesAsync();

        Task RefreshNewlyAddedPackageVersionsAsync();
    }
}