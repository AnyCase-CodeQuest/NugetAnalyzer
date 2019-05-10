using System.Collections.Generic;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IFileService
    {
        string[] GetFilesPaths(string directoryPath, string searchPattern);

        string GetFilePath(string directoryPath, string fileSearchPattern);

        string GetFileContent(string filePath);

        IList<string> GetFilesDirectoriesPaths(string[] filesPaths);
    }
}