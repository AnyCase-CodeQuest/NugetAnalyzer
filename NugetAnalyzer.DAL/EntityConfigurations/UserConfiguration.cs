using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder
                .Property(p => p.Email)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(p => p.UserName)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}