using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly NugetAnalyzerDbContext context;
        private readonly IServiceProvider serviceProvider;

        public UnitOfWork(NugetAnalyzerDbContext context, IServiceProvider serviceProvider)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IRepositoriesRepository RepositoriesRepository => (IRepositoriesRepository)GetRepository<Repository>();
        public IPackageVersionsRepository PackageVersionsRepository => (IPackageVersionsRepository)GetRepository<PackageVersion>();
        public IProjectsRepository ProjectsRepository => (IProjectsRepository)GetRepository<Project>();

        public IRepository<T> GetRepository<T>()
            where T : class
        {
            return serviceProvider.GetRequiredService<IRepository<T>>();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}