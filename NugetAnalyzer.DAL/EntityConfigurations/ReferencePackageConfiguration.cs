using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class ReferencePackageConfiguration : IEntityTypeConfiguration<ReferencePackage>
    {
        public void Configure(EntityTypeBuilder<ReferencePackage> builder)
        {
            builder
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(4096);
        }
    }
}