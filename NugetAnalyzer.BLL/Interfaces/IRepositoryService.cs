using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryService
    {
        Task<ICollection<RepositoryWithVersionReport>> GetUserAnalyzedRepositoriesAsync(int userId);
    }
}
