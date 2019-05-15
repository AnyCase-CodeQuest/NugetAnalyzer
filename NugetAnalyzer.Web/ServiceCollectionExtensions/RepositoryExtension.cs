using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.DAL.Repositories;
using NugetAnalyzer.DAL.UnitOfWork;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.Web.ServiceCollectionExtensions
{
    public static class RepositoryExtension
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepositoryRepository, RepositoryRepository>();
            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IRepository<Repository>), provider => provider.GetService<IRepositoryRepository>());
            services.AddScoped(typeof(IRepository<PackageVersion>), provider => provider.GetService<IVersionRepository>());
        }
    }
}
