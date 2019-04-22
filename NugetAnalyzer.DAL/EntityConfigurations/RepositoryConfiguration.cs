using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class RepositoryConfiguration : IEntityTypeConfiguration<Repository>
    {
        public void Configure(EntityTypeBuilder<Repository> builder)
        {
            builder
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(4096);

            builder
                .Property(p => p.UserId)
                .IsRequired();
        }
    }
}