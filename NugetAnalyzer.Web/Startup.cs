using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models.Configurations;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.DAL.Repositories;
using NugetAnalyzer.DAL.UnitOfWork;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Web.ApplicationBuilderExtensions;
using NugetAnalyzer.Web.Middleware;
using NugetAnalyzer.Web.ServiceCollectionExtensions;

namespace NugetAnalyzer.Web
{
    public class Startup
    {
        private const string ResourcePath = "Resources";
        public Startup(IHostingEnvironment environment)
        {
            Configuration = new ConfigurationBuilder()
                                .SetBasePath(environment.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: false)
                                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                                .Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<NugetAnalyzerDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString:DefaultConnection"]);
            });

            services.Configure<PackageVersionConfiguration>(options => Configuration.GetSection("PackageStatus").Bind(options));
            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

            services.Configure<NugetSettings>(Configuration.GetSection("NugetEndpoints"));
            services.AddSingleton<INugetApiService, NugetApiService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepositoryRepository, RepositoryRepository>();
            services.AddScoped<IVersionAnalyzerService, VersionAnalyzerService>();
            services.AddScoped<IRepositoryService, RepositoryService>();

            services.AddScoped(typeof(IRepository<Repository>), provider => provider.GetService<IRepositoryRepository>());
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ISourceService, SourceService>();

            var secretsSection = Configuration.GetSection("GitHubAppSettings");
            var endPointsSection = Configuration.GetSection("GitHubEndPoints");
            services.AddGitHubOAuth(endPointsSection, secretsSection);
            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped<INugetService, NugetService>();
            services.AddScoped<IVersionService, VersionService>();
            services.AddScoped<IPackageService, PackageService>();

            services.AddScoped(
                typeof(IRepository<PackageVersion>),
                provider => provider.GetService<IVersionRepository>());

            services.AddHttpClient<INugetHttpService, NugetHttpService>();
            services.AddMvc().AddViewLocalization(p => p.ResourcesPath = ResourcePath);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseConfiguredLocalization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}