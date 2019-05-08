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
        private readonly IDirectoryService directoryService;
        private readonly IFileService fileService;

        public RepositoryAnalyzerService(IDirectoryService directoryService, IFileService fileService)
        {
            this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public Repository GetParsedRepository(string repositoryPath)
        {
            if (!directoryService.IsDirectoryExist(repositoryPath))
            {
                return null;
            }

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

            foreach (var solutionDirectoryPath in directoryService.GetDirectoriesPaths(solutionsFilesPaths))
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

            foreach (var projectDirectoryPath in directoryService.GetDirectoriesPaths(projectsFilesPaths))
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
            if (fileService.GetPackagesConfigFilePath(project.Path) != null)
            {
                project.Packages = GetPackagesOfFrameworkApp(fileService.GetPackagesConfigFilePath(project.Path));
            }
            else
            {
                project.Packages = GetPackagesOfCoreApp(fileService.GetCsProjFilePath(project.Path));
            }
        }

        private IList<Package> GetPackagesOfCoreApp(string csProjFilePath)
        {
            IList<Package> packages = new List<Package>();

            XmlDocument document = new XmlDocument();

            document.LoadXml(fileService.GetFileContent(csProjFilePath));

            XmlNodeList nodesList = document.SelectNodes("//Project/ItemGroup/PackageReference");

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
                else
                {
                    packages.Add(
                         new Package
                         {
                             Name = node.Attributes["Include"].Value,
                             Version = null
                         });
                }
            }

            return packages;
        }

        private IList<Package> GetPackagesOfFrameworkApp(string packagesConfigFilePath)
        {
            IList<Package> packages = new List<Package>();

            XmlDocument document = new XmlDocument();

            document.LoadXml(fileService.GetFileContent(packagesConfigFilePath));

            XmlNodeList nodesList = document.SelectNodes("//packages/package");

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
                else
                {
                    packages.Add(
                       new Package
                       {
                           Name = node.Attributes["id"].Value,
                           Version = null
                       });
                }
            }

            return packages;
        }
    }
}