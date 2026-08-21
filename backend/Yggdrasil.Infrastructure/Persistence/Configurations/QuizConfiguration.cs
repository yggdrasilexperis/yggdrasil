using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Yggdrasil.Domain.Entities;
using Yggdrasil.Infrastructure.Identity;

namespace Yggdrasil.Infrastructure.Persistence.Configurations;

public sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.Property(q => q.Title).IsRequired().HasMaxLength(200);
        builder.Property(q => q.Description).HasMaxLength(2000);
        builder.Property(q => q.Difficulty).IsRequired();
        builder.Property(q => q.OwnerId).IsRequired();
        builder.Property(q => q.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
        builder.Property(q => q.UpdatedAt).HasColumnType("timestamp with time zone"); ;

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(q => q.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.Questions)
            .WithOne(qu => qu.Quiz)
            .HasForeignKey(qu => qu.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Comments)
            .WithOne(c => c.Quiz)
            .HasForeignKey(c => c.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Categories)
            .WithMany(c => c.Quizzes)
            .UsingEntity<Dictionary<string, object>>(
                "QuizCategory",
                j => j.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Quiz>()
                    .WithMany()
                    .HasForeignKey("QuizId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("QuizCategories");
                    j.HasKey("QuizId", "CategoryId");
                });
    }
}
