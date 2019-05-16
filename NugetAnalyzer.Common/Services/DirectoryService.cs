using System;
using System.IO;
using NugetAnalyzer.Common.Interfaces;

namespace NugetAnalyzer.Common.Services
{
    public class DirectoryService : IDirectoryService
    {
        public bool Exists(string path)
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

        public void Create(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            Directory.CreateDirectory(path);
        }

        public void Delete(string path)
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