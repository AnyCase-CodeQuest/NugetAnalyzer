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
                .Property(user => user.Email)
                .HasMaxLength(256)
                .IsRequired();

            builder
                .Property(user => user.UserName)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}