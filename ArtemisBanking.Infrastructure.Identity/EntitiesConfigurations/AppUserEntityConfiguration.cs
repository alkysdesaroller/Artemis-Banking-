using ArtemisBanking.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infrastructure.Identity.EntitiesConfigurations;

public class AppUserEntityConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(a =>  a.FirstName).IsRequired().HasMaxLength(200);
        builder.Property(a =>  a.LastName).IsRequired().HasMaxLength(200);
    }
}