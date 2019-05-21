using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectReport> GetProjectReportAsync(int projectId);
    }
}