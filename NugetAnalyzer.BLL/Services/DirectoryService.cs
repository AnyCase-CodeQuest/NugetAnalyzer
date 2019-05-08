using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
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

        public string GetDirectoryName(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            return new DirectoryInfo(directoryPath).Name;
        }

        public IList<string> GetDirectoriesPaths(string[] filesPaths)
        {
            IList<string> directoriesPaths = new List<string>();

            foreach (var filePath in filesPaths)
            {
                FileInfo file = new FileInfo(filePath);

                directoriesPaths.Add(file.DirectoryName);
            }

            return directoriesPaths;
        }

        public string CreateDirectoryForRepository()
        {
            var rootDirectory = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            var repositoryPath = Guid.NewGuid().ToString();

            while (IsDirectoryExist(rootDirectory + "\\" + repositoryPath))
            {
                repositoryPath = Guid.NewGuid().ToString();
            }

            Directory.CreateDirectory(rootDirectory + "\\" + repositoryPath);

            return repositoryPath;
        }

        public void DeleteRepository(string path)
        {
            if (IsDirectoryExist(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}