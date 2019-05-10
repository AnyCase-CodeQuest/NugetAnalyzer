using System;
using System.Xml;
using System.Collections.Generic;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.BLL.Models.Solutions;
using NugetAnalyzer.BLL.Models.Projects;
using NugetAnalyzer.BLL.Models.Packages;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryAnalyzerService : IRepositoryAnalyzerService
    {
        private enum FrameworkType
        {
            Core,
            Framework
        }

        private readonly IDirectoryService directoryService;
        private readonly IFileService fileService;

        public RepositoryAnalyzerService(IDirectoryService directoryService, IFileService fileService)
        {
            this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public Repository GetParsedRepository(string repositoryPath)
        {
            if (!directoryService.IsDirectoryExists(repositoryPath))
                return null;

            Repository repository = new Repository
            {
                Name = directoryService.GetDirectoryName(repositoryPath),
                Path = repositoryPath,
                Solutions = new List<Solution>()
            };

            AddSolutionsToRepository(repository);

            return repository;
        }

        private void AddSolutionsToRepository(Repository repository)
        {
            var solutionsFilesPaths = fileService.GetFilesPaths(repository.Path, "*.sln");

            foreach (var solutionDirectoryPath in fileService.GetFilesDirectoriesPaths(solutionsFilesPaths))
            {
                Solution solution = new Solution
                {
                    Name = directoryService.GetDirectoryName(solutionDirectoryPath),
                    Path = solutionDirectoryPath,
                    Projects = new List<Project>()
                };

                AddProjectsToSolution(solution);

                repository.Solutions.Add(solution);
            }
        }

        private void AddProjectsToSolution(Solution solution)
        {
            var projectsFilesPaths = fileService.GetFilesPaths(solution.Path, "*.csproj");

            foreach (var projectDirectoryPath in fileService.GetFilesDirectoriesPaths(projectsFilesPaths))
            {
                Project project = new Project
                {
                    Name = directoryService.GetDirectoryName(projectDirectoryPath),
                    Path = projectDirectoryPath,
                    Packages = new List<Package>()
                };

                AddPackagesToProject(project);

                solution.Projects.Add(project);
            }
        }

        private void AddPackagesToProject(Project project)
        {
            if (fileService.GetFilePath(project.Path, "packages.config") != null)
            {
                var filePath = fileService.GetFilePath(project.Path, "packages.config");

                project.Packages = GetPackages(FrameworkType.Framework, filePath);
            }
            else
            {
                var filePath = fileService.GetFilePath(project.Path, "*.csproj");

                project.Packages = GetPackages(FrameworkType.Core, filePath);
            }
        }

        private IList<Package> GetPackages(FrameworkType frameworkType, string filePath)
        {
            var packages = new List<Package>();
            var document = new XmlDocument();

            document.LoadXml(fileService.GetFileContent(filePath));

            var nodesList = GetXmlNodeList(document, frameworkType);

            AddPackagesToPackagesList(frameworkType, packages, nodesList);

            return packages;
        }

        private XmlNodeList GetXmlNodeList(XmlDocument document, FrameworkType frameworkType)
        {
            switch (frameworkType)
            {
                case FrameworkType.Core:
                    return document.SelectNodes("//Project/ItemGroup/PackageReference");

                case FrameworkType.Framework:
                    return document.SelectNodes("//packages/package");

                default:
                    return null;
            }
        }

        private void AddPackagesToPackagesList(FrameworkType frameworkType, IList<Package> packages, XmlNodeList nodesList)
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

        private void AddPackagesToPackagesListForCoreApp(IList<Package> packages, XmlNodeList nodesList)
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

        private void AddPackagesToPackagesListForFrameworkApp(IList<Package> packages, XmlNodeList nodesList)
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