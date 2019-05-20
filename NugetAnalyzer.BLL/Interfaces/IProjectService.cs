using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectReportDTO> GetProjectReport(int projectId);
    }
}