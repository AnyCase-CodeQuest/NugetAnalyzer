using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryAnalyzerService
    {
        public Repository GetParsedRepository(string repositoryPath)
        {
            Repository repository = new Repository
            {
                Name = GetDirectoryMame(repositoryPath),
                Path = repositoryPath
            };

            foreach (var solutionDirectoryPath in GetSolutionsDirectoriesPaths(repository.Path))
            {
                Solution solution = new Solution
                {
                    Name = GetDirectoryMame(solutionDirectoryPath),
                    Path = solutionDirectoryPath
                };

                foreach (var projectDirectoryPath in GetProjectsDirectoriesPaths(solution.Path))
                {
                    Project project = new Project
                    {
                        Name = GetDirectoryMame(projectDirectoryPath),
                        Path = projectDirectoryPath
                    };

                    if (GetPackagesConfigPath(project.Path) != null)
                    {
                        project.Packages = GetPackagesOfFrameworkApp(GetPackagesConfigPath(project.Path));
                    }
                    else
                    {
                        project.Packages = GetPackagesOfCoreApp(GetCsProjFilePath(project.Path));
                    }

                    solution.Projects.Add(project);
                }

                repository.Solutions.Add(solution);
            }
            
            return repository;
        }

        private string GetCsProjFilePath(string projectFolderPath)
        {
            var csProjPath = Directory.GetFiles(projectFolderPath, "*.csproj", SearchOption.AllDirectories);

            return csProjPath.Count() == 0 ? null : csProjPath[0];
        }

        private string GetPackagesConfigPath(string projectPath)
        {
            var packageConfigPath = Directory.GetFiles(projectPath, "packages.config", SearchOption.AllDirectories);

            return packageConfigPath.Count() == 0 ? null : packageConfigPath[0];
        }

        private static string GetDirectoryMame(string directoryPath)
        {
            return new DirectoryInfo(directoryPath).Name;
        }

        private static IList<string> GetSolutionsDirectoriesPaths(string repositoryPath)
        {
            string[] solutionsFilesPaths = Directory.GetFiles(repositoryPath, "*.sln", SearchOption.AllDirectories);

            return GetFoldersPaths(solutionsFilesPaths);
        }

        private static IList<string> GetProjectsDirectoriesPaths(string solutionPath)
        {
            string[] projectsFilesPaths = Directory.GetFiles(solutionPath, "*.csproj", SearchOption.AllDirectories);

            return GetFoldersPaths(projectsFilesPaths);
        }

        private static IList<string> GetFoldersPaths(string[] pathsArray)
        {
            IList<string> paths = new List<string>();

            foreach (var path in pathsArray)
            {
                FileInfo file = new FileInfo(path);

                paths.Add(file.DirectoryName);
            }

            return paths;
        }

        private IList<Package> GetPackagesOfCoreApp(string csProjFilePath)
        {
            IList<Package> packages = new List<Package>();

            XmlDocument doc = CreateaXmlDocument(csProjFilePath);

            XmlNodeList nodesList = doc.SelectNodes("//Project/ItemGroup/PackageReference");

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

            XmlDocument doc = CreateaXmlDocument(packagesConfigFilePath);

            XmlNodeList nodesList = doc.SelectNodes("//packages/package");

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

        private XmlDocument CreateaXmlDocument(string filePath)
        {
            XmlDocument document = new XmlDocument();

            document.Load(filePath);

            return document;
        }
    }



    public class Repository
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Solution> Solutions { get; set; }
    }

    public class Solution
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Project> Projects { get; set; }
    }

    public class Project
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Package> Packages { get; set; }
    }

    public class Package
    {
        public string Name { get; set; }

        public string Version { get; set; }
    }
}