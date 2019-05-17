using System.Threading.Tasks;
using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositorySaverService
    {
        Task SaveAsync(Repository repository, int userId);
    }
}
