﻿using Microsoft.AspNetCore.Builder;
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

            services.Configure<NugetSettings>(Configuration.GetSection("NugetEndpoints"));

            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextInfoProvider>();

            IConfigurationSection gitHubEndPointsSection = Configuration.GetSection("GitHubEndPoints");
            IConfigurationSection gitHubAppSettingsSection = Configuration.GetSection("GitHubAppSettings");
            services.AddGitHubOAuth(gitHubEndPointsSection, gitHubAppSettingsSection);
            services.Configure<PackageVersionConfigurations>(options => Configuration.GetSection("PackageStatus").Bind(options));

            services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddSingleton<INugetApiService, NugetApiService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INugetService, NugetService>();
            services.AddScoped<IVersionService, VersionService>();
            services.AddScoped<IPackageService, PackageService>();

            services.AddScoped<IPackageVersionsRepository, PackageVersionsRepository>();
            services.AddScoped<IRepositoriesRepository, RepositoriesRepository>();
            services.AddScoped<IVersionsAnalyzerService, VersionsAnalyzerService>();
            services.AddScoped<IRepositoryService, RepositoryService>();

            services.AddScoped(typeof(IRepository<PackageVersion>), provider => provider.GetService<IPackageVersionsRepository>());
            services.AddScoped(typeof(IRepository<Repository>), provider => provider.GetService<IRepositoriesRepository>());

            services.AddHttpClient<INugetHttpService, NugetHttpService>();
            services.AddMvc();

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