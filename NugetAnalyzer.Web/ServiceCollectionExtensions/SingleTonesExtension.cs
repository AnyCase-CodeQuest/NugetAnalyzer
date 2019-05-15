using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.Common.Services;

namespace NugetAnalyzer.Web.ServiceCollectionExtensions
{
    public static class SingleTonesExtension
    {
        public static void AddSingleToneItemts(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddSingleton<INugetApiService, NugetApiService>();
            services.AddSingleton<IDirectoryService, DirectoryService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IRepositoryAnalyzerService, RepositoryAnalyzerService>();
        }
    }
}
