using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositorySaverService
    {
        Task SaveAsync(RepositoryDTO repositoryDTO, int userId);
    }
}
