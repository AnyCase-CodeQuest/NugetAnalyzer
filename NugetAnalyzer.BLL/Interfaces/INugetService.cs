using System.Threading.Tasks;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface INugetService
    {
        Task RefreshLatestVersionOfAllPackagesAsync();

        Task RefreshLatestVersionOfNewPackagesAsync();
    }
}