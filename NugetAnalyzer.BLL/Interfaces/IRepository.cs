using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepository
    {
        void Save(Repository repository, int userId);
    }
}
