using System.Collections.Generic;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IDirectoryService
    {
        bool IsDirectoryExist(string repositoryPath);

        string GetDirectoryName(string directoryPath);

        IList<string> GetDirectoriesPaths(string[] filesPaths);
    }
}