using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.DAL.Repositories;
using NugetAnalyzer.DAL.UnitOfWork;
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

            services.Configure<NugetSettings>(Configuration.GetSection("NugetEndpoints"));

            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddSingleton<INugetApiService, NugetApiService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped<INugetService, NugetService>();
            services.AddScoped<IVersionService, VersionService>();

            services.AddScoped(
                typeof(IRepository<PackageVersion>),
                provider => provider.GetService<IVersionRepository>());

            services.AddHttpClient<INugetHttpService, NugetHttpService>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseStaticFiles();
            app.UseMvc();
            
            app.UseMvcWithDefaultRoute();
        }
    }
}