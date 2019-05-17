using System.Threading.Tasks;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface ISourceService
    {
        Task<int> GetSourceIdByName(string sourceName);
    }
}
