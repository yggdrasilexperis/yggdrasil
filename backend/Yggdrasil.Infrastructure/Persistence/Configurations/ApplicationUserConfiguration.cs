using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Yggdrasil.Infrastructure.Identity;

namespace Yggdrasil.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.UserName).IsRequired();
        builder.Property(u => u.NormalizedUserName).IsRequired();
        builder.Property(u => u.Email).IsRequired();
        builder.Property(u => u.NormalizedEmail).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasIndex(u => u.NormalizedEmail).IsUnique().HasDatabaseName("EmailIndex");
    }
}