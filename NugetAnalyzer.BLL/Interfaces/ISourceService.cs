using System.Threading.Tasks;
using NugetAnalyzer.Domain.Enums;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface ISourceService
    {
        Task<int> GetSourceIdByName(SourceType sourceType);
    }
}
