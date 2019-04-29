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
                .HasMaxLength(256);

            builder
                .Property(p => p.UserName)
                .IsRequired()
                .HasMaxLength(256);

            builder
                .Property(p => p.FirstName)
                .HasMaxLength(256);

            builder
                .Property(p => p.LastName)
                .HasMaxLength(256);
        }
    }
}