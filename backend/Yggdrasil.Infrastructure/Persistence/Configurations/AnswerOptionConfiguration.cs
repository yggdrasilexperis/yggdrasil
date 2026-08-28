using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Yggdrasil.Domain.Entities;

namespace Yggdrasil.Infrastructure.Persistence.Configurations;

public sealed class AnswerOptionConfiguration : IEntityTypeConfiguration<AnswerOption>
{
    public void Configure(EntityTypeBuilder<AnswerOption> builder)
    {
        builder.Property(a => a.Text).IsRequired().HasMaxLength(500);
        builder.Property(a => a.IsCorrect).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired().HasColumnType("timestamp with time zone"); ;
    }
}
