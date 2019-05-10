using System;
using System.IO;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class DirectoryService : IDirectoryService
    {
        public bool IsDirectoryExists(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Directory.Exists(path);
        }

        public string GetDirectoryName(string directoryPath)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            return new DirectoryInfo(directoryPath).Name;
        }

        public void CreateDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}