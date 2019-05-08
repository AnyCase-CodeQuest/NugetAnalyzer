using System;
using System.Collections.Generic;
using System.Linq;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public static class RepositoryMapper
    {
        public static Repository ToDomain(this Models.Repositories.Repository businessRepository, int userId,
            IUnitOfWork database)
        {
            Repository domainRepository = CreateRepository(businessRepository.Name, userId);
            foreach (var solution in businessRepository.Solutions)
            {
                domainRepository.Solutions.Add(CreateSolution(solution.Name));
                foreach (var project in solution.Projects)
                {
                    var domainSolution = domainRepository.Solutions.FirstOrDefault(o => o.Name == solution.Name);
                    domainSolution.Projects.Add(CreateProject(project.Name));

                    foreach (var package in project.Packages)
                    {
                        var domainPackage = domainSolution.Projects.FirstOrDefault(o => o.Name == project.Name)
                            .ProjectPackageVersions;
                        domainPackage.Add(CreatePackage(domainSolution, database, package.Name, package.Version,
                            domainSolution.Projects.FirstOrDefault(o => o.Name == project.Name)));
                    }
                }
            }

            return domainRepository;
        }

        private static Repository CreateRepository(string name, int userId)
        {
            return new Repository
            {
                Name = name,
                UserId = userId,
                Solutions = new List<Solution>()
            };
        }

        private static Solution CreateSolution(string name)
        {
            return new Solution
            {
                Name = name,
                Projects = new List<Project>(),
            };
        }

        private static Project CreateProject(string name)
        {
            return new Project
            {
                Name = name,
                ProjectPackageVersions = new List<ProjectPackageVersion>()
            };
        }

        private static ProjectPackageVersion CreatePackage(Solution domainSolution, IUnitOfWork unitOfWork,
            string packageName, string version, Project domainProject, Repository domainRepository)
        {
            Package package = unitOfWork.GetRepository<Package>().GetSingleOrDefaultAsync(o => o.Name == packageName)
                .Result;
            Version packageVersion = CreatePackageVersion(version);

            if (package == null && !IsContainsPackageWithName(domainRepository, packageName))
            {
                return new ProjectPackageVersion
                {
                    Project = domainSolution.Projects.FirstOrDefault(o => o.Name == domainProject.Name),
                    PackageVersion = new PackageVersion
                    {
                        Package = new Package {Name = packageName},
                        Minor = packageVersion.Minor,
                        Major = packageVersion.Major,
                        Build = packageVersion.Build,
                        Revision = packageVersion.Revision
                    }
                };
            }

            if (package.Versions.FirstOrDefault(o => o.Build == packageVersion.Build) == null)
            {
                return new ProjectPackageVersion
                {
                    Project = domainSolution.Projects.FirstOrDefault(o => o.Name == domainProject.Name),
                    PackageVersion = new PackageVersion
                    {
                        Package = package,
                        Minor = packageVersion.Minor,
                        Major = packageVersion.Major,
                        Build = packageVersion.Build,
                        Revision = packageVersion.Revision
                    }
                };
            }

            return new ProjectPackageVersion
            {
                Project = domainSolution.Projects.FirstOrDefault(o => o.Name == domainProject.Name),
                PackageVersion = unitOfWork.GetRepository<PackageVersion>()
                    .GetSingleOrDefaultAsync(o => o.Package.Name == package.Name).Result
            };
        }

        private static Version CreatePackageVersion(string version)
        {
            Version packageVersion = new Version();
            Version.TryParse(version, out packageVersion);
            return packageVersion;
        }

        private static bool IsContainsPackageWithName(Repository repository, string name)
        {
            foreach (var solution in repository.Solutions)
            {
                foreach (var project in solution.Projects)
                {
                    foreach (var projectPackageVersion in project.ProjectPackageVersions)
                    {
                        if (projectPackageVersion.PackageVersion.Package.Name == name)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}