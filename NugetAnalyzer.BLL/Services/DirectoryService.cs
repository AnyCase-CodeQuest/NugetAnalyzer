using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class DirectoryService : IDirectoryService
    {
        public bool IsDirectoryExist(string repositoryPath)
        {
            if (repositoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryPath));
            }

            return Directory.Exists(repositoryPath);
        }

        public string GetDirectoryMame(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            return new DirectoryInfo(directoryPath).Name;
        }

        public IList<string> GetSolutionsDirectoriesPaths(string repositoryPath)
        {
            var solutionsFilesPaths = GetFilesPaths(repositoryPath, "*.sln");

            return GetDirectoriesPaths(solutionsFilesPaths);
        }

        public IList<string> GetProjectsDirectoriesPaths(string solutionPath)
        {
            var projectsFilesPaths = GetFilesPaths(solutionPath, "*.csproj");

            return GetDirectoriesPaths(projectsFilesPaths);
        }

        public string GetPackagesConfigFilePath(string projectPath)
        {
            var packageConfigFilePath = GetFilesPaths(projectPath, "packages.config");

            return packageConfigFilePath.Count() == 0 ? null : packageConfigFilePath[0];
        }

        public string GetCsProjFilePath(string projectDirectoryPath)
        {
            var csProjFilePath = GetFilesPaths(projectDirectoryPath, "*.csproj");

            return csProjFilePath.Count() == 0 ? null : csProjFilePath[0];
        }

        private IList<string> GetDirectoriesPaths(string[] filesPaths)
        {
            IList<string> directoriesPaths = new List<string>();

            foreach (var filePath in filesPaths)
            {
                FileInfo file = new FileInfo(filePath);

                directoriesPaths.Add(file.DirectoryName);
            }

            return directoriesPaths;
        }

        private string[] GetFilesPaths(string directoryPath, string searchPattern)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            if (searchPattern == null)
                throw new ArgumentNullException(nameof(searchPattern));

            return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
        }
    }
}