using System;
using System.Collections.Generic;
using System.Linq;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryMapper
    {
        private IRepository<Repository> DatabaseRepository { get; }
        private IRepository<Package> PackageRepository { get; }
        private IRepository<PackageVersion> PackageVersionRepository { get; }

        public RepositoryMapper(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("Unit of work not initialized.");
            }

            DatabaseRepository = unitOfWork.GetRepository<Repository>();
            PackageRepository = unitOfWork.GetRepository<Package>();
            PackageVersionRepository = unitOfWork.GetRepository<PackageVersion>();
        }

        public Repository ToDomain(Models.Repositories.Repository businessRepository, int userId)
        {
            Repository dbRepository = DatabaseRepository.GetSingleOrDefaultAsync(o => o.Name == businessRepository.Name)
                .Result;
            if (dbRepository != null)
            {
                DatabaseRepository.Delete(dbRepository.Id);
            }

            Repository domainRepository = CreateRepository(businessRepository.Name, userId);

            foreach (var solution in businessRepository.Solutions)
            {
                domainRepository.Solutions.Add(CreateSolution(solution.Name));
                Solution domainSolution = domainRepository.Solutions.FirstOrDefault(o => o.Name == solution.Name);

                foreach (var project in solution.Projects)
                {
                    domainSolution.Projects.Add(CreateProject(project.Name));
                    Project domainProject = domainSolution.Projects.FirstOrDefault(o => o.Name == project.Name);

                    foreach (var package in project.Packages)
                    {
                        domainProject.ProjectPackageVersions.Add(CreatePackage(package.Name, package.Version,
                            domainProject, domainRepository));
                    }
                }
            }

            return domainRepository;
        }

        private Repository CreateRepository(string name, int userId)
        {
            return new Repository
            {
                Name = name,
                UserId = userId,
                Solutions = new List<Solution>()
            };
        }

        private Solution CreateSolution(string name)
        {
            return new Solution
            {
                Name = name,
                Projects = new List<Project>(),
            };
        }

        private Project CreateProject(string name)
        {
            return new Project
            {
                Name = name,
                ProjectPackageVersions = new List<ProjectPackageVersion>()
            };
        }

        private ProjectPackageVersion CreatePackage(
            string packageName, string version, Project domainProject, Repository domainRepository)
        {
            Package package = PackageRepository.GetSingleOrDefaultAsync(o => o.Name == packageName)
                .Result;
            Version packageVersion = CreatePackageVersion(version);
            PackageVersion tempPackageVersion;
            Package tempPackage;

            if (package != null)
            {
                tempPackageVersion = PackageVersionRepository.GetSingleOrDefaultAsync(o =>
                    o.Package.Name == packageName && o.Major == packageVersion.Major &&
                    o.Minor == packageVersion.Minor && o.Build == packageVersion.Build &&
                    o.Revision == packageVersion.Revision).Result;
                if (tempPackageVersion != null)
                {
                    return new ProjectPackageVersion
                    {
                        Project = domainProject,
                        PackageVersion = tempPackageVersion
                    };
                }

                tempPackageVersion = new PackageVersion
                {
                    Package = package,
                    Minor = packageVersion.Minor,
                    Major = packageVersion.Major,
                    Build = packageVersion.Build,
                    Revision = packageVersion.Revision
                };
                package.Versions.Add(tempPackageVersion);
                return new ProjectPackageVersion
                {
                    Project = domainProject,
                    PackageVersion = tempPackageVersion
                };
            }

            package = ReturnExistingPackage(domainRepository, packageName);
            if (package != null)
            {
                tempPackageVersion = ReturnExistingPackageVersion(domainRepository, packageName, packageVersion);
                if (tempPackageVersion != null)
                {
                    return new ProjectPackageVersion
                    {
                        Project = domainProject,
                        PackageVersion = tempPackageVersion
                    };
                }

                tempPackageVersion = new PackageVersion
                {
                    Package = package,
                    Minor = packageVersion.Minor,
                    Major = packageVersion.Major,
                    Build = packageVersion.Build,
                    Revision = packageVersion.Revision
                };
                package.Versions.Add(tempPackageVersion);
                return new ProjectPackageVersion
                {
                    Project = domainProject,
                    PackageVersion = tempPackageVersion
                };
            }

            tempPackageVersion = new PackageVersion
            {
                Minor = packageVersion.Minor,
                Major = packageVersion.Major,
                Build = packageVersion.Build,
                Revision = packageVersion.Revision
            };
            tempPackage = new Package
            {
                Name = packageName,
                Versions = new List<PackageVersion>()
            };
            tempPackage.Versions.Add(tempPackageVersion);
            tempPackageVersion.Package = tempPackage;
            return new ProjectPackageVersion
            {
                Project = domainProject,
                PackageVersion = tempPackageVersion
            };
        }

        private Version CreatePackageVersion(string version)
        {
            Version packageVersion = new Version();
            Version.TryParse(version, out packageVersion);
            return packageVersion;
        }

        private Package ReturnExistingPackage(Repository repository, string name)
        {
            foreach (var solution in repository.Solutions)
            {
                foreach (var project in solution.Projects)
                {
                    foreach (var projectPackageVersion in project.ProjectPackageVersions)
                    {
                        if (projectPackageVersion.PackageVersion.Package.Name == name)
                        {
                            return projectPackageVersion.PackageVersion.Package;
                        }
                    }
                }
            }

            return null;
        }

        private PackageVersion ReturnExistingPackageVersion(Repository repository, string name, Version version)
        {
            foreach (var solution in repository.Solutions)
            {
                foreach (var project in solution.Projects)
                {
                    foreach (var projectPackageVersion in project.ProjectPackageVersions)
                    {
                        PackageVersion tempPackageVersion = projectPackageVersion.PackageVersion;
                        if (tempPackageVersion.Package.Name == name &&
                            tempPackageVersion.Minor == version.Minor &&
                            tempPackageVersion.Major == version.Major &&
                            tempPackageVersion.Build == version.Build &&
                            tempPackageVersion.Revision == version.Revision)
                        {
                            return projectPackageVersion.PackageVersion;
                        }
                    }
                }
            }

            return null;
        }
    }
}