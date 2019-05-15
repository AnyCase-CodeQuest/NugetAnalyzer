using System.Threading.Tasks;
using NugetAnalyzer.Dtos;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectViewModel> GetProjectById(int projectId);
    }
}
