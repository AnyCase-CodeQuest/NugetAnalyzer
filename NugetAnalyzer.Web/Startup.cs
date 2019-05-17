using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.Common;
using NugetAnalyzer.Common.Configurations;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.Web.Infrastructure.Extensions;
using NugetAnalyzer.Web.Infrastructure.HttpAccessors;
using NugetAnalyzer.Web.Middleware;

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
            services.AddConverters();

            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextInfoProvider>();

            IConfigurationSection gitHubEndPointsSection = Configuration.GetSection("GitHubEndPoints");
            IConfigurationSection gitHubAppSettingsSection = Configuration.GetSection("GitHubAppSettings");
            services.AddGitHubOAuth(gitHubEndPointsSection, gitHubAppSettingsSection);
            services.Configure<PackageVersionConfigurations>(options => Configuration.GetSection("PackageStatus").Bind(options));
            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddNugetAnalyzerRepositories();
            services.AddNugetAnalyzerServices();
            services.AddMvc().AddViewLocalization(p => p.ResourcesPath = ResourcePath);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/ServerError");
                app.UseStatusCodePagesWithRedirects("/Error/NotFoundError");
            }

            app.UseConfiguredLocalization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}