using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.EntityConfigurations;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Context
{
    public class NugetAnalyzerDbContext : DbContext
    {
        public NugetAnalyzerDbContext(DbContextOptions<NugetAnalyzerDbContext> options)
            :base(options)
        {
        }

        public DbSet<Package> Packages { get; set; }
        public DbSet<ReferencePackage> ReferencePackages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PackageConfiguration());
            builder.ApplyConfiguration(new ReferencePackageConfiguration());
            builder.ApplyConfiguration(new ProjectConfiguration());
            builder.ApplyConfiguration(new RepositoryConfiguration());
            builder.ApplyConfiguration(new SolutionConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}