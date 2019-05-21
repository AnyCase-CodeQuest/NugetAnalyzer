using System;
using System.IO;
using Microsoft.Extensions.Options;
using NugetAnalyzer.Common.Configurations;
using NugetAnalyzer.Common.Interfaces;

namespace NugetAnalyzer.Common.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly DirectoriesConfiguration directoriesConfiguration;
        public DirectoryService(IOptions<DirectoriesConfiguration> directoriesConfiguration)
        {
            if (directoriesConfiguration == null)
            {
                throw new ArgumentNullException(nameof(directoriesConfiguration));
            }
            this.directoriesConfiguration = directoriesConfiguration.Value;
        }

        public bool Exists(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return Directory.Exists(path);
        }

        public string GetName(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            return new DirectoryInfo(directoryPath).Name;
        }

        public string GenerateClonePath()
        {
            return directoriesConfiguration.RepositoryCloneDirectory + "/" + Guid.NewGuid().ToString();
        }

        public void Create(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Directory.CreateDirectory(path);
        }

        public void Delete(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (!Directory.Exists(path))
            {
                return;
            }

            var filesNames = Directory.GetFiles(path);
            var directoriesNames = Directory.GetDirectories(path);

            foreach (var fileName in filesNames)
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
            }

            foreach (var directoryName in directoriesNames)
            {
                Delete(directoryName);
            }

            File.SetAttributes(path, FileAttributes.Normal);
            Directory.Delete(path, false);
        }
    }
}