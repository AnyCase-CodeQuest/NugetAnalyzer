using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepository
    {
        void SaveAsync(Repository repository, int userId);
    }
}
