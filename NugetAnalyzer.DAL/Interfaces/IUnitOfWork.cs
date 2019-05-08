using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        object GetRepository<T>()
            where T : class;

        Task<int> SaveChangesAsync();
    }
}