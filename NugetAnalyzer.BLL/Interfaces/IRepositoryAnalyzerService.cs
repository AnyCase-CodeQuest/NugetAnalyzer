using NugetAnalyzer.BLL.Models.Repositories;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryAnalyzerService
    {
        Repository GetParsedRepository(string repositoryPath);
    }
}