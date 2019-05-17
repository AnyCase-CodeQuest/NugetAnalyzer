using System.Threading.Tasks;
using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryAnalyzerService
    {
        Task<Repository> GetParsedRepositoryAsync(string repositoryPath);
    }
}