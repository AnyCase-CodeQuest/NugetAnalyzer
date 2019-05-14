using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryService
    {
        void SaveAsync(Repository repository, int userId);
    }
}
