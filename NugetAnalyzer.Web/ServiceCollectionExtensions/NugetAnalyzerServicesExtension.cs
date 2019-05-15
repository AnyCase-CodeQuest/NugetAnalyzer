using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;

namespace NugetAnalyzer.Web.ServiceCollectionExtensions
{
    public static class NugetAnalyzerServicesExtension
    {
        public static void AddNugetAnalyzerServices(this IServiceCollection services)
        {
            services.AddScoped<IVersionAnalyzerService, VersionAnalyzerService>();
            services.AddScoped<IRepositoryService, RepositoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ISourceService, SourceService>();
            services.AddScoped<INugetService, NugetService>();
            services.AddScoped<IVersionService, VersionService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IRepositoryServiceSanek, RepositoryServiceSanek>();
            services.AddHttpClient<INugetHttpService, NugetHttpService>();
        }
    }
}
