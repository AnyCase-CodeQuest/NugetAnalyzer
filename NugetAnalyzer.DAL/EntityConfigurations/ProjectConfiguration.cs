using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(4096);

            builder
                .Property(p => p.SolutionId)
                .IsRequired();
        }
    }
}