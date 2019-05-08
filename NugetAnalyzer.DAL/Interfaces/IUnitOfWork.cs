using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IVersionRepository VersionRepository { get; }

        IRepository<T> GetRepository<T>()
            where T : class;

        Task<int> SaveChangesAsync();
    }
}