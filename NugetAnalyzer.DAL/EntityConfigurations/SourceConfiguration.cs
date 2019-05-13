using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class SourceConfiguration : IEntityTypeConfiguration<Source>
    {
        public void Configure(EntityTypeBuilder<Source> builder)
        {
            builder.ToTable("Sources");

            builder
                .Property(p => p.Name)
                .HasMaxLength(64)
                .IsRequired(true);
        }
    }
}
