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
            this.hostingEnvironment = hostingEnvironment;
        }

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

        public string CreateDirectoryForRepository()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(hostingEnvironment.WebRootPath);
            stringBuilder.Append("\\");
            stringBuilder.Append(Guid.NewGuid().ToString());

            while (IsDirectoryExist(stringBuilder.ToString()))
            {
                stringBuilder = new StringBuilder();

                stringBuilder.Append(hostingEnvironment.WebRootPath);
                stringBuilder.Append("\\");
                stringBuilder.Append(Guid.NewGuid().ToString());
            }

            Directory.CreateDirectory(stringBuilder.ToString());

            return stringBuilder.ToString();
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