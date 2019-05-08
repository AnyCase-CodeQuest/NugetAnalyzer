using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryService
    {
        ICollection<RepositoryWithVersionReport> GetUserAnalyzedRepositories(int userId);

    }
}
