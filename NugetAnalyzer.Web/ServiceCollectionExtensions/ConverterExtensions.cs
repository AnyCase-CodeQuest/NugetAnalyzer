using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.Dtos.Converters;

namespace NugetAnalyzer.Web.ServiceCollectionExtensions
{
    public static class ConverterExtensions
    {
        public static void AddConverters(this IServiceCollection services)
        {
            services.AddScoped<ProfileConverter>();
            services.AddScoped<UserConverter>();
        }
    }
}
