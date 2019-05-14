using System.Collections.Generic;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Packages;
using NugetAnalyzer.BLL.Models.Projects;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.BLL.Models.Solutions;
using NugetAnalyzer.DAL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepository
    {
        private IUnitOfWork UnitOfWork { get; }

        public RepositoryService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public void Save(Repository repository, int userId)
        {
            Repository repository2 = new Repository
            {
                Name = "AreaMonitor",
                Solutions = new List<Solution>
                {
                    new Solution
                    {
                        Name = "AreaMonitor.UI",
                        Projects = new List<Project>
                        {
                            new Project
                            {
                                Name = "AreaMonitor.UI",
                                Packages = new List<Package>
                                {
                                    new Package {Name = "DiffPlex", Version = "1.4.4"},
                                    new Package {Name = "Microsoft.AspNetCore.Http.Abstractions", Version = "2.2.0"},
                                    new Package {Name = "Microsoft.AspNetCore.Razor.Design", Version = "2.2.0"},
                                    new Package {Name = "Microsoft.VisualStudio.Web.CodeGeneration.Design", Version = "2.2.0"},
                                    new Package {Name = "NLog.Web.AspNetCore", Version = "4.8.1"},
                                }
                            },
                            new Project
                            {
                                Name = "AreaMonitor.UI.BL",
                                Packages = new List<Package>
                                {
                                    new Package {Name = "System.Text.Encoding.CodePages", Version = "4.5.1"},
                                }
                            }
                        }
                    },
                    new Solution
                    {
                        Name = "AreaMonitor.WebApi",
                        Projects = new List<Project>
                        {
                            new Project
                            {
                                Name = "AreaMonitor.WebApi",
                                Packages = new List<Package>
                                {
                                    new Package {Name = "DiffPlex", Version = "1.4.4"},
                                    new Package {Name = "Microsoft.AspNetCore.Http.Abstractions", Version = "2.2.0"},
                                    new Package {Name = "Microsoft.AspNetCore.Razor.Design", Version = "2.2.0"},
                                    new Package {Name = "Microsoft.VisualStudio.Web.CodeGeneration.Design", Version = "2.2.0"},
                                    new Package {Name = "NLog.Web.AspNetCore", Version = "4.8.1"},
                                }
                            },
                            new Project
                            {
                                Name = "AreaMonitor.WebApi.BL",
                                Packages = new List<Package>
                                {
                                    new Package {Name = "DiffPlex", Version = "1.4.2"},
                                    new Package {Name = "Hangfire", Version = "1.6.23"},
                                    new Package {Name = "HtmlAgilityPack", Version = "1.11.1"},
                                    new Package {Name = "Newtonsoft.Json", Version = "12.0.1"},
                                }
                            },
                            new Project
                            {
                                Name = "AreaMonitor.WebApi.BL.Tests",
                                Packages = new List<Package>
                                {
                                    new Package {Name = "Microsoft.NET.Test.Sdk", Version = "16.0.1"},
                                    new Package {Name = "Moq", Version = "4.10.1"},
                                    new Package {Name = "NUnit", Version = "3.11.0"},
                                    new Package {Name = "NUnit.ConsoleRunner", Version = "3.10.0"},
                                    new Package {Name = "NUnit3TestAdapter", Version = "3.13.0"},
                                }
                            },
                            new Project
                            {
                                Name = "AreaMonitor.WebApi.DAL",
                                Packages = new List<Package>
                                {
                                    new Package {Name = "Microsoft.AspNetCore.Http.Abstractions", Version = "2.2.0"},
                                    new Package {Name = "Microsoft.AspNetCore.Identity.EntityFrameworkCore", Version = "2.2.0"},
                                    new Package {Name = "Microsoft.EntityFrameworkCore.SqlServer", Version = "2.2.3"},
                                    new Package {Name = "Microsoft.Extensions.Identity.Stores", Version = "2.2.0"},
                                }
                            },
                            new Project
                            {
                                Name = "AreaMonitor.WebApi.Domain",
                                Packages = new List<Package>
                                {
                                    new Package {Name = "Microsoft.AspNetCore.Identity", Version = "2.2.0"},
                                    new Package {Name = "Microsoft.Extensions.Identity.Stores", Version = "2.2.0"},
                                }
                            }
                        }
                    }
                }
            };
            RepositoryMapper mapper = new RepositoryMapper(UnitOfWork);
            UnitOfWork.GetRepository<Domain.Repository>().Add(mapper.ToDomain(repository2, userId));
            UnitOfWork.SaveChangesAsync();
        }
    }
}
