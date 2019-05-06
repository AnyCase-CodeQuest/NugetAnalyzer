using NugetAnalyzer.BLL.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryAnalyzerService
    {
        Repository GetParsedRepository(string repositoryPath);
    }
}