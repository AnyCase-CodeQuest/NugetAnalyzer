using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common;
using NugetAnalyzer.Common.Configurations;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.DAL.Repositories;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Web.Middleware;

namespace NugetAnalyzer.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment)
        {
            Configuration = new ConfigurationBuilder()
                                .SetBasePath(environment.ContentRootPath)
                                .AddJsonFile("appsettings.json")
                                .Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<NugetAnalyzerDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString:DefaultConnection"]);
            });

            services.Configure<PackageVersionConfigurations>(options => Configuration.GetSection("PackageStatus").Bind(options));
            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPackageVersionsRepository, PackageVersionsRepository>();
            services.AddScoped<IRepositoriesRepository, RepositoriesRepository>();
            services.AddScoped<IVersionsAnalyzerService, VersionsAnalyzerService>();
            services.AddScoped<IRepositoryService, RepositoryService>();

            services.AddScoped(typeof(IRepository<PackageVersion>), provider => provider.GetService<IPackageVersionsRepository>());
            services.AddScoped(typeof(IRepository<Repository>), provider => provider.GetService<IRepositoriesRepository>());

            services.AddMvc();
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

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}