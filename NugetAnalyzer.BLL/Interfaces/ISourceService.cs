using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface ISourceService
    {
        Task<int> GetSourceIdByName(string sourceName);
    }
}
