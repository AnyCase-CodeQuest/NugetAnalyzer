using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IRepositoryRepository
    {
        Task<IReadOnlyCollection<Repository>> GetUserRepositoriesWithIncludes(int userId);
    }
}
