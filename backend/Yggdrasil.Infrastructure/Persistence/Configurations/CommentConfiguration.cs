using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Yggdrasil.Domain.Entities;
using Yggdrasil.Infrastructure.Identity;

namespace Yggdrasil.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(c => c.Body).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.AuthorId).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone"); ;
        builder.Property(c => c.UpdatedAt).HasColumnType("timestamp with time zone"); ;

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
