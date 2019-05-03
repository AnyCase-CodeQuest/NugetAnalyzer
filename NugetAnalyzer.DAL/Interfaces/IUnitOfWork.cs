using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> GetGenericRepository<T>()
            where T : class;

        T GetRepository<T>()
            where T : IVersionRepository;

        Task<int> SaveChangesAsync();
    }
}