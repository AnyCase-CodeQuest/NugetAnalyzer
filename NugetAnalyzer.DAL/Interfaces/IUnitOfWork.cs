using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IVersionRepository VersionRepository { get; }

        IRepository<T> GetRepository<T>()
            where T : class;

        IRepositoryRepository RepositoryRepository { get; }

        IVersionRepository VersionRepository { get; }

        Task<int> SaveChangesAsync();
    }
}