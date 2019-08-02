using System;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositorySaverService : IRepositorySaverService
    {
        private readonly IUnitOfWork unitOfWork;

        private IRepository<Repository> databaseRepository;
        private IRepository<PackageVersion> packageVersionRepository;
        private IRepository<Package> packageRepository;

        private IRepository<Repository> DatabaseRepository
        {
            get
            {
                if (databaseRepository == null)
                {
                    databaseRepository = unitOfWork.GetRepository<Repository>();
                }

                return databaseRepository;
            }
        }

        private IRepository<Package> PackageRepository
        {
            get
            {
                if (packageRepository == null)
                {
                    packageRepository = unitOfWork.GetRepository<Package>();
                }

                return packageRepository;
            }
        }

        private IRepository<PackageVersion> PackageVersionRepository
        {
            get
            {
                if (packageVersionRepository == null)
                {
                    packageVersionRepository = unitOfWork.GetRepository<PackageVersion>();
                }

                return packageVersionRepository;
            }
        }

        public RepositorySaverService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task SaveAsync(RepositoryDTO repositoryDTO, int userId)
        {
            var repository = await ToDomainAsync(repositoryDTO, userId);
            unitOfWork.GetRepository<Repository>().Add(repository);
            await unitOfWork.SaveChangesAsync();
        }

        #region PrivateMethods

        private async Task<Repository> ToDomainAsync(RepositoryDTO businessRepositoryDTO, int userId)
        {
            var dbRepository =
                await DatabaseRepository.GetSingleOrDefaultAsync(repository =>
                    repository.Name == businessRepositoryDTO.Name);
            if (dbRepository != null)
            {
                DatabaseRepository.Delete(dbRepository.Id);
            }

            var domainRepository = CreateRepository(businessRepositoryDTO.Name, userId);

            foreach (var businessSolution in businessRepositoryDTO.Solutions)
            {
                domainRepository.Solutions.Add(CreateSolution(businessSolution.Name));
                var domainSolution =
                    domainRepository.Solutions.FirstOrDefault(solution => solution.Name == businessSolution.Name);

                foreach (var businessProject in businessSolution.Projects)
                {
                    domainSolution.Projects.Add(CreateProject(businessProject.Name));
                    var domainProject =
                        domainSolution.Projects.FirstOrDefault(project => project.Name == businessProject.Name);

                    foreach (var businessPackage in businessProject.Packages)
                    {
                        domainProject.ProjectPackageVersions.Add(await CreatePackageAsync(businessPackage.Name,
                            businessPackage.Version,
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
                UserId = userId
            };
        }

        private Solution CreateSolution(string name)
        {
            return new Solution
            {
                Name = name
            };
        }

        private Project CreateProject(string name)
        {
            return new Project
            {
                Name = name
            };
        }

        private async Task<ProjectPackageVersion> CreatePackageAsync(
            string packageName,
            string version,
            Project domainProject,
            Repository domainRepository)
        {
            var package = await PackageRepository.GetSingleOrDefaultAsync(dbPackage => dbPackage.Name == packageName);
            var packageVersion = CreatePackageVersion(version);
            PackageVersion tempPackageVersion;

            if (package != null)
            {
                tempPackageVersion = await PackageVersionRepository.GetSingleOrDefaultAsync(dbPackageVersion =>
                    dbPackageVersion.Package.Name == packageName
                    && dbPackageVersion.Major == packageVersion.Major
                    && dbPackageVersion.Minor == packageVersion.Minor
                    && dbPackageVersion.Build == packageVersion.Build
                    && dbPackageVersion.Revision == packageVersion.Revision);
                if (tempPackageVersion != null)
                {
                    return new ProjectPackageVersion
                    {
                        Project = domainProject,
                        PackageVersion = tempPackageVersion
                    };
                }

                tempPackageVersion = CreatePackageVersion(packageVersion, package);
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

                tempPackageVersion = CreatePackageVersion(packageVersion, package);
                package.Versions.Add(tempPackageVersion);
                return new ProjectPackageVersion
                {
                    Project = domainProject,
                    PackageVersion = tempPackageVersion
                };
            }

            tempPackageVersion = CreatePackageVersion(packageVersion);
            var tempPackage = new Package
            {
                Name = packageName
            };
            tempPackage.Versions.Add(tempPackageVersion);
            tempPackageVersion.Package = tempPackage;
            return new ProjectPackageVersion
            {
                Project = domainProject,
                PackageVersion = tempPackageVersion
            };
        }

        private PackageVersion CreatePackageVersion(Version version, Package package = null)
        {
            return new PackageVersion
            {
                Package = package,
                Minor = version.Minor,
                Major = version.Major,
                Build = version.Build,
                Revision = version.Revision
            };
        }

        private Version CreatePackageVersion(string version)
        {
            Version.TryParse(version, out var packageVersion);
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
                        var tempPackageVersion = projectPackageVersion.PackageVersion;
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

        #endregion
    }
}