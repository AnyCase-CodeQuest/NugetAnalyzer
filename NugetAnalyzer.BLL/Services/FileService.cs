using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class FileService : IFileService
    {
        public string[] GetFilesPaths(string directoryPath, string searchPattern)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            if (searchPattern == null)
                throw new ArgumentNullException(nameof(searchPattern));

            return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
        }

        public string GetPackagesConfigFilePath(string projectPath)
        {
            var packageConfigFilePaths = GetFilesPaths(projectPath, "packages.config");

            return packageConfigFilePaths.Count() == 0 ? null : packageConfigFilePaths[0];
        }

        public string GetCsProjFilePath(string projectPath)
        {
            var csProjFilePaths = GetFilesPaths(projectPath, "*.csproj");

            return csProjFilePaths.Count() == 0 ? null : csProjFilePaths[0];
        }

        public string GetFileContent(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return streamReader.ReadToEnd();
            }
        }

        public IList<string> GetFilesDirectoriesPaths(string[] filesPaths)
        {
            IList<string> directoriesPaths = new List<string>();

            foreach (var filePath in filesPaths)
            {
                FileInfo file = new FileInfo(filePath);

                directoriesPaths.Add(file.DirectoryName);
            }

            return directoriesPaths;
        }
    }
}