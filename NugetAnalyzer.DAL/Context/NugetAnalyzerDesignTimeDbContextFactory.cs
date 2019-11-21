using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NugetAnalyzer.DAL.Context
{
    public class NugetAnalyzerDesignTimeDbContextFactory : IDesignTimeDbContextFactory<NugetAnalyzerDbContext>
    {
        public NugetAnalyzerDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Directory.GetCurrentDirectory() + "/../NugetAnalyzer.Web/appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<NugetAnalyzerDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new NugetAnalyzerDbContext(builder.Options);
        }
    }
}
