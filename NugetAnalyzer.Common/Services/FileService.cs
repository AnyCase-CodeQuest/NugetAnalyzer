using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using NugetAnalyzer.Common.Interfaces;

namespace NugetAnalyzer.Common.Services
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

        public string GetFilePath(string directoryPath, string searchPattern)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            if (searchPattern == null)
                throw new ArgumentNullException(nameof(searchPattern));

            var filesPaths = GetFilesPaths(directoryPath, searchPattern);

            return filesPaths.Count() == 0 ? null : filesPaths[0];
        }

        public Task<string> GetContentAsync(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return streamReader.ReadToEndAsync();
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
    }
}