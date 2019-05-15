using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.EntityConfigurations
{
    class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");

            builder
                .Property(profile => profile.SourceId)
                .IsRequired();

            builder
                .Property(profile => profile.UserId)
                .IsRequired();

            builder
                .Property(profile => profile.AccessToken)
                .IsRequired();

            builder
                .Property(profile => profile.Url)
                .IsRequired();
        }
    }
}
