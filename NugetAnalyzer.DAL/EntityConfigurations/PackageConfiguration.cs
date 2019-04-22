using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasKey(p => p.PackageId);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(4096);

            builder
                .HasOne(p => p.ReferencePackage)
                .WithMany(p => p.CurrentPackages)
                .HasForeignKey(p => p.ReferencePackageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}