using System.Threading.Tasks;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositorySaverService
    {
        Task<Repository> SaveAsync(RepositoryDTO repositoryDTO, int userId);
    }
}
