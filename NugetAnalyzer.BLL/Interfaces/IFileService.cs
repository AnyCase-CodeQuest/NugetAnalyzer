using System.Collections.Generic;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IFileService
    {
        string[] GetFilesPaths(string directoryPath, string searchPattern);

        string GetPackagesConfigFilePath(string projectPath);

        string GetCsProjFilePath(string projectPath);

        string GetFileContent(string filePath);

        IList<string> GetFilesDirectoriesPaths(string[] filesPaths);
    }
}