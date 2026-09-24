using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Infrastructure.Identity;

namespace Yggdrasil.Infrastructure.Persistence.Seeding;

public class DatabaseSeeder(YggdrasilDbContext db, IOptions<SeedOptions> options) : IDatabaseSeeder
{
    private const string SeedPassword = "Password123!";

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var password = options.Value.Password;
        if (string.IsNullOrWhiteSpace(password))
        {
            // TODO: need to update hardcoded seed password to correspond with env later on
            throw new InvalidOperationException(
                $"{SeedOptions.SectionName}:{nameof(SeedOptions.Password)} is missing. Locally, you need to have in user secrets:\n "
                    + "  dotnet user-secrets set \"Seed:Password\" \"Password123!\" "
                    + "--project backend/Yggdrasil.Api\n"
                    + "For a hosted database, set the Seed__Password environment variable instead."
            );
        }

        if (await db.Users.AnyAsync(u => u.Id == SeedData.AlvaId, ct))
            return;

        var alva = CreateUser(SeedData.AlvaId, "alva", "alva@example.com", password);
        var jonas = CreateUser(SeedData.JonasId, "jonas", "jonas@example.com", password);
        var admin = CreateUser(SeedData.AdminId, "admin", "admin@example.com", password);
        db.Users.AddRange(alva, jonas, admin);

        db.UserRoles.AddRange(
            new IdentityUserRole<Guid> { UserId = alva.Id, RoleId = SeedData.UserRoleId },
            new IdentityUserRole<Guid> { UserId = jonas.Id, RoleId = SeedData.UserRoleId },
            new IdentityUserRole<Guid> { UserId = admin.Id, RoleId = SeedData.AdminRoleId }
        );

        var categories = SeedData.Categories();
        db.Categories.AddRange(categories);
        db.Quizzes.AddRange(SeedData.Quizzes(categories));
        db.Comments.AddRange(SeedData.Comments());

        await db.SaveChangesAsync(ct);
    }

    private static ApplicationUser CreateUser(
        Guid id,
        string userName,
        string email,
        string password
    )
    {
        var user = new ApplicationUser()
        {
            Id = id,
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        user.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(user, password);
        return user;
    }
}
