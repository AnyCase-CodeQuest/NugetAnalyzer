using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class PackageVersionConfiguration : IEntityTypeConfiguration<PackageVersion>
    {
        public void Configure(EntityTypeBuilder<PackageVersion> builder)
        {
            builder.ToTable("PackageVersions");

            builder
                .HasOne(p => p.Package)
                .WithMany(p => p.Versions)
                .HasForeignKey(p => p.PackageId);
        }
    }
}