using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>()
            where T : class;

        IRepositoryRepository RepositoryRepository { get; }

        IVersionRepository VersionRepository { get; }

        Task<int> SaveChangesAsync();
    }
}