using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Yggdrasil.Domain.Constants;
using Yggdrasil.Infrastructure.Persistence.Seeding;

namespace Yggdrasil.Infrastructure.Persistence.Configurations;

public sealed class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = SeedData.AdminRoleId,
                Name = Roles.Admin,
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "0a1b7f2c-0000-4000-8000-0000000000b1"
            },
            new IdentityRole<Guid>
            {
                Id = SeedData.UserRoleId,
                Name = Roles.User,
                NormalizedName = "USER",
                ConcurrencyStamp = "0a1b7f2c-0000-4000-8000-0000000000b2"
            });
    }
}
