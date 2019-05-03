using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>()
            where T : class;

        IVersionRepository GetVersionRepository();

        Task<int> SaveChangesAsync();
    }
}