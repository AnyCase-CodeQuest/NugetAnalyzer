using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IProjectsRepository : IRepository<Project>
    {
        Task<Project> GetByIdWithIncludedPackageAsync(int id);
    }
}