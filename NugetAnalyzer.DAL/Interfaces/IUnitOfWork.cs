using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IUsersRepository UsersRepository { get; }

        IRepository<T> GetRepository<T>()
            where T : class;

        Task<int> SaveChangesAsync();
    }
}