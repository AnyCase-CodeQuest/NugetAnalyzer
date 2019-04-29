using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class ProjectPackageVersionConfiguration : IEntityTypeConfiguration<ProjectPackageVersion>
    {
        public void Configure(EntityTypeBuilder<ProjectPackageVersion> builder)
        {
            builder.ToTable("ProjectPackageVersions");

            builder
                .HasKey(p => new {p.ProjectId, p.PackageVersionId});

            builder
                .HasOne(p => p.Project)
                .WithMany(p => p.ProjectPackageVersions)
                .HasForeignKey(p => p.ProjectId);

            builder
                .HasOne(p => p.PackageVersion)
                .WithMany(p => p.ProjectPackageVersions)
                .HasForeignKey(p => p.PackageVersionId);
        }
    }
}