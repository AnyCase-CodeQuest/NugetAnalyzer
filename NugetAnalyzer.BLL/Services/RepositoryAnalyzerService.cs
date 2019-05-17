using System;
using System.Xml;
using System.Threading.Tasks;
using System.Collections.Generic;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.BLL.Models.Solutions;
using NugetAnalyzer.BLL.Models.Projects;
using NugetAnalyzer.BLL.Models.Packages;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryAnalyzerService : IRepositoryAnalyzerService
    {
        private const string SolutionSearchPattern = "*.sln";
        private const string CsProjSearchPattern = "*.csproj";
        private const string PackagesConfigSearchPattern = "packages.config";

        private readonly IDirectoryService directoryService;
        private readonly IFileService fileService;

        public RepositoryAnalyzerService(IDirectoryService directoryService, IFileService fileService)
        {
            this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        private enum FrameworkType
        {
            Core = 0,
            Framework = 1
        }

        public async Task<Repository> GetParsedRepositoryAsync(string repositoryPath)
        {
            if (!directoryService.Exists(repositoryPath))
            {
                return null;
            }

            var repository = new Repository
            {
                Name = directoryService.GetName(repositoryPath),
                Path = repositoryPath
            };

            await AddSolutionsToRepositoryAsync(repository);

            return repository;
        }

        private async Task AddSolutionsToRepositoryAsync(Repository repository)
        {
            var solutionsFilesPaths = fileService.GetFilesPaths(repository.Path, SolutionSearchPattern);

            foreach (var solutionDirectoryPath in fileService.GetFilesDirectoriesPaths(solutionsFilesPaths))
            {
                var solution = new Solution
                {
                    Name = directoryService.GetName(solutionDirectoryPath),
                    Path = solutionDirectoryPath
                };

                await AddProjectsToSolutionAsync(solution);

                repository.Solutions.Add(solution);
            }
        }

        private async Task AddProjectsToSolutionAsync(Solution solution)
        {
            var projectsFilesPaths = fileService.GetFilesPaths(solution.Path, CsProjSearchPattern);

            foreach (var projectDirectoryPath in fileService.GetFilesDirectoriesPaths(projectsFilesPaths))
            {
                var project = new Project
                {
                    Name = directoryService.GetName(projectDirectoryPath),
                    Path = projectDirectoryPath
                };

                await AddPackagesToProjectAsync(project);

                solution.Projects.Add(project);
            }
        }

        private async Task AddPackagesToProjectAsync(Project project)
        {
            if (fileService.GetFilePath(project.Path, PackagesConfigSearchPattern) != null)
            {
                var filePath = fileService.GetFilePath(project.Path, PackagesConfigSearchPattern);

                project.Packages = await GetPackagesAsync(FrameworkType.Framework, filePath);
            }
            else
            {
                var filePath = fileService.GetFilePath(project.Path, CsProjSearchPattern);

                project.Packages = await GetPackagesAsync(FrameworkType.Core, filePath);
            }
        }

        private async Task<ICollection<Package>> GetPackagesAsync(FrameworkType frameworkType, string filePath)
        {
            var packages = new List<Package>();
            var document = new XmlDocument();

            var fileContent = await fileService.GetContentAsync(filePath);
            document.LoadXml(fileContent);

            var nodesList = GetXmlNodeListWithPackagesOfXmlDocument(document, frameworkType);

            AddPackagesToPackagesList(frameworkType, packages, nodesList);

            return packages;
        }

        private XmlNodeList GetXmlNodeListWithPackagesOfXmlDocument(XmlDocument document, FrameworkType frameworkType)
        {
            switch (frameworkType)
            {
                case FrameworkType.Core:
                {
                    return document.SelectNodes("//Project/ItemGroup/PackageReference");
                }
                case FrameworkType.Framework:
                {
                    return document.SelectNodes("//packages/package");
                }
                default:
                {
                    return null;
                }
            }
        }

        private void AddPackagesToPackagesList(FrameworkType frameworkType, ICollection<Package> packages, XmlNodeList nodesList)
        {
            switch (frameworkType)
            {
                case FrameworkType.Core:
                {
                    AddPackagesToPackagesListForCoreApp(packages, nodesList);
                    break;
                }
                case FrameworkType.Framework:
                {
                    AddPackagesToPackagesListForFrameworkApp(packages, nodesList);
                    break;
                }
            }
        }

        private void AddPackagesToPackagesListForCoreApp(ICollection<Package> packages, XmlNodeList nodesList)
        {
            foreach (XmlNode node in nodesList)
            {
                if (node.Attributes["Version"] != null)
                {
                    packages.Add(
                        new Package
                        {
                            Name = node.Attributes["Include"].Value,
                            Version = node.Attributes["Version"].Value
                        });
                }
            }
        }

        private void AddPackagesToPackagesListForFrameworkApp(ICollection<Package> packages, XmlNodeList nodesList)
        {
            foreach (XmlNode node in nodesList)
            {
                if (node.Attributes["version"] != null)
                {
                    packages.Add(
                        new Package
                        {
                            Name = node.Attributes["id"].Value,
                            Version = node.Attributes["version"].Value
                        });
                }
            }
        }
    }
}