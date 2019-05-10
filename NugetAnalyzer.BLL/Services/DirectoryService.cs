using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public DirectoryService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        public bool IsDirectoryExist(string repositoryPath)
        {
            if (repositoryPath == null)
                throw new ArgumentNullException(nameof(repositoryPath));

            return Directory.Exists(repositoryPath);
        }

        public string GetDirectoryName(string directoryPath)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(nameof(directoryPath));

            return new DirectoryInfo(directoryPath).Name;
        }

        public string CreateDirectoryForRepository()
        {
            StringBuilder path = new StringBuilder();

            path = this.CreateNewPath(path);

            while (IsDirectoryExist(path.ToString()))
            {
                path = this.CreateNewPath(path);
            }

            Directory.CreateDirectory(path.ToString());

            return path.ToString();
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

        private StringBuilder CreateNewPath(StringBuilder stringBuilder)
        {
            stringBuilder.Clear();

            stringBuilder.Append(hostingEnvironment.WebRootPath);
            stringBuilder.Append("\\");
            stringBuilder.Append(Guid.NewGuid().ToString());

            return stringBuilder;
        }
    }
}