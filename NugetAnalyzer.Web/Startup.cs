using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.DAL.Repositories;
using NugetAnalyzer.DAL.UnitOfWork;
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
            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ISourceService, SourceService>();

            var secretsSection = Configuration.GetSection("GitHubAppSettings");
            var endPointsSection = Configuration.GetSection("GitHubEndPoints");
            services.AddGitHubOAuth(endPointsSection, secretsSection);

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