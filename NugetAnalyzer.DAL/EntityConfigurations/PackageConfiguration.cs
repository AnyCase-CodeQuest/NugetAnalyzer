using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasKey(k => k.PackageId);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(4096);

            builder
                .HasOne(x => x.ReferencePackage)
                .WithMany(x => x.CurrentPackages)
                .HasForeignKey(x => x.ReferencePackageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}