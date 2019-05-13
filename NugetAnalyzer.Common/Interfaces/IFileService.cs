using System.Threading.Tasks;
using System.Collections.Generic;

namespace NugetAnalyzer.Common.Interfaces
{
    public interface IFileService
    {
        string[] GetFilesPaths(string directoryPath, string searchPattern);

        string GetFilePath(string directoryPath, string searchPattern);

        Task<string> GetFileContentAsync(string filePath);

        IList<string> GetFilesDirectoriesPaths(string[] filesPaths);
    }
}