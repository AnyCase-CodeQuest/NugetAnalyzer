using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.EntityConfigurations;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Context
{
    public class NugetAnalyzerDbContext : DbContext
    {
        public NugetAnalyzerDbContext(DbContextOptions<NugetAnalyzerDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PackageConfiguration());
            builder.ApplyConfiguration(new PackageVersionConfiguration());
            builder.ApplyConfiguration(new ProjectPackageVersionConfiguration());
            builder.ApplyConfiguration(new ProjectConfiguration());
            builder.ApplyConfiguration(new RepositoryConfiguration());
            builder.ApplyConfiguration(new SolutionConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new SourceConfiguration());
            builder.ApplyConfiguration(new ProfileConfiguration());
        }
    }
}