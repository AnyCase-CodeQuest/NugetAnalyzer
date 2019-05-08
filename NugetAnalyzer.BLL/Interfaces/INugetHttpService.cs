using System.Threading.Tasks;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface INugetHttpService
    {
        Task<string> GetPackageMetadataAsync(string packageName, string version);

        Task<string> GetDataOfPackageVersionAsync(string query);
    }
}