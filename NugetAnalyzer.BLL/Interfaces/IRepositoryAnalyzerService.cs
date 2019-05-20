using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryAnalyzerService
    {
        Task<RepositoryDTO> GetParsedRepositoryAsync(string repositoryPath);
    }
}