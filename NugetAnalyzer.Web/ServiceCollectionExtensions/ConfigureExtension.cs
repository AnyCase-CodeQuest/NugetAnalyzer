using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.Common;
using NugetAnalyzer.Dtos.Models.Configurations;

namespace NugetAnalyzer.Web.ServiceCollectionExtensions
{
    public static class ConfigureExtension
    {
        public static void AddConfigure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PackageVersionConfiguration>(configuration.GetSection("PackageStatus"));
            services.Configure<NugetSettings>(configuration.GetSection("NugetEndpoints"));
        }
    }
}
