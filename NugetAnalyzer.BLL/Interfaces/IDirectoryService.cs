using System.Collections.Generic;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IDirectoryService
    {
        bool IsDirectoryExist(string repositoryPath);

        string GetDirectoryMame(string directoryPath);

        IList<string> GetSolutionsDirectoriesPaths(string repositoryPath);

        IList<string> GetProjectsDirectoriesPaths(string solutionPath);

        string GetPackagesConfigFilePath(string projectPath);

        string GetCsProjFilePath(string projectDirectoryPath);
    }
}