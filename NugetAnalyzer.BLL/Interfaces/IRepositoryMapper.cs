using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryMapper
    {
        Task<Repository> ToDomainAsync(Models.Repositories.Repository businessRepository, int userId);
    }
}
