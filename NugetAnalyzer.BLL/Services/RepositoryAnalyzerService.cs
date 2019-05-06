using System;
using System.Xml;
using System.Collections.Generic;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryAnalyzerService : IRepositoryAnalyzerService
    {
        private readonly IDirectoryService directoryService;

        public RepositoryAnalyzerService(IDirectoryService directoryService)
        {
            this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
        }

        public Repository GetParsedRepository(string repositoryPath)
        {
            if (!directoryService.IsDirectoryExist(repositoryPath))
            {
                return null;
            }

            Repository repository = new Repository
            {
                Name = directoryService.GetDirectoryMame(repositoryPath),
                Path = repositoryPath,
                Solutions = new List<Solution>()
            };

            AddSolutionsToRepository(repository);

            return repository;
        }

        private void AddSolutionsToRepository(Repository repository)
        {
            foreach (var solutionDirectoryPath in directoryService.GetSolutionsDirectoriesPaths(repository.Path))
            {
                Solution solution = new Solution
                {
                    Name = directoryService.GetDirectoryMame(solutionDirectoryPath),
                    Path = solutionDirectoryPath,
                    Projects = new List<Project>()
                };

                AddProjectsToSolution(solution);

                repository.Solutions.Add(solution);
            }
        }

        private void AddProjectsToSolution(Solution solution)
        {
            foreach (var projectDirectoryPath in directoryService.GetProjectsDirectoriesPaths(solution.Path))
            {
                Project project = new Project
                {
                    Name = directoryService.GetDirectoryMame(projectDirectoryPath),
                    Path = projectDirectoryPath,
                    Packages = new List<Package>()
                };

                AddPackagesToProject(project);

                solution.Projects.Add(project);
            }
        }

        private void AddPackagesToProject(Project project)
        {
            if (directoryService.GetPackagesConfigFilePath(project.Path) != null)
            {
                project.Packages = GetPackagesOfFrameworkApp(directoryService.GetPackagesConfigFilePath(project.Path));
            }
            else
            {
                project.Packages = GetPackagesOfCoreApp(directoryService.GetCsProjFilePath(project.Path));
            }
        }

        private IList<Package> GetPackagesOfCoreApp(string csProjFilePath)
        {
            IList<Package> packages = new List<Package>();

            XmlDocument document = GetXmlDocument(csProjFilePath);

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

            XmlDocument document = GetXmlDocument(packagesConfigFilePath);

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

        private XmlDocument GetXmlDocument(string filePath)
        {
            XmlDocument document = new XmlDocument();

            document.Load(filePath);

            return document;
        }
    }
}