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
                .HasKey(projectPackageVersion => new { projectPackageVersion.ProjectId, projectPackageVersion.PackageVersionId});

            builder
                .HasOne(projectPackageVersion => projectPackageVersion.Project)
                .WithMany(project => project.ProjectPackageVersions)
                .HasForeignKey(projectPackageVersion => projectPackageVersion.ProjectId);

            builder
                .HasOne(projectPackageVersion => projectPackageVersion.PackageVersion)
                .WithMany(packageVersion => packageVersion.ProjectPackageVersions)
                .HasForeignKey(projectPackageVersion => projectPackageVersion.PackageVersionId);
        }
    }
}