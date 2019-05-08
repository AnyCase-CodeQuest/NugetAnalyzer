using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
            if (projectPath == null)
                throw new ArgumentNullException(nameof(projectPath));

            return GetFilePath(GetFilesPaths(projectPath, "packages.config"));
        }

        public string GetCsProjFilePath(string projectPath)
        {
            if (projectPath == null)
                throw new ArgumentNullException(nameof(projectPath));

            return GetFilePath(GetFilesPaths(projectPath, "*.csproj"));
        }

        public string GetFileContent(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return streamReader.ReadToEnd();
            }
        }

        public IList<string> GetFilesDirectoriesPaths(string[] filesPaths)
        {
            if (filesPaths == null)
                throw new ArgumentNullException(nameof(filesPaths));

            IList<string> directoriesPaths = new List<string>();

            foreach (var filePath in filesPaths)
            {
                FileInfo file = new FileInfo(filePath);

                directoriesPaths.Add(file.DirectoryName);
            }

            return directoriesPaths;
        }

        private string GetFilePath(string[] filesPaths)
        {
            return filesPaths.Count() == 0 ? null : filesPaths[0];
        }
    }
}