using System.Threading.Tasks;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface INugetHttpService
    {
        Task<string> GetPackageVersionMetadataAsync(string packageName, string version);

        Task<string> GetPackageMetadataAsync(string query);
    }
}