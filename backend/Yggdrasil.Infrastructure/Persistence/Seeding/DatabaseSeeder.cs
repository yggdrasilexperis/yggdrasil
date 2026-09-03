using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Infrastructure.Identity;

namespace Yggdrasil.Infrastructure.Persistence.Seeding;

public class DatabaseSeeder(YggdrasilDbContext db) : IDatabaseSeeder
{
    private const string SeedPassword = "Password123!";

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Users.AnyAsync(u => u.Id == SeedData.AlvaId, ct)) return;

        db.Roles.AddRange(
            new IdentityRole<Guid> { Id = SeedData.AdminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole<Guid> { Id = SeedData.UserRoleId, Name = "User", NormalizedName = "USER" }
            );

        var alva = CreateUser(SeedData.AlvaId, "alva", "alva@example.com");
        var jonas = CreateUser(SeedData.JonasId, "jonas", "jonas@example.com");
        var admin = CreateUser(SeedData.AdminId, "admin", "admin@example.com");
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

    private static ApplicationUser CreateUser(Guid id, string userName, string email)
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
        user.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(user, SeedPassword);
        return user;
    }
}
