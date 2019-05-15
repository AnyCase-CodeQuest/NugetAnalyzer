using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryServiceSanek
    {
        void SaveAsync(Repository repository, int userId);
    }
}
