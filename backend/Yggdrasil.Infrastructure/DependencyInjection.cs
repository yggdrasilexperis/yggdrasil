using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Options;
using Yggdrasil.Application.Services;
using Yggdrasil.Infrastructure.Identity;
using Yggdrasil.Infrastructure.Persistence;
using Yggdrasil.Infrastructure.Persistence.Seeding;
using Yggdrasil.Infrastructure.Repositories;

namespace Yggdrasil.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:Postgres is missing. Locally, store it in user secrets; "
                    + "elsewhere, set the ConnectionStrings__Postgres environment variable."
            );
        }

        services.AddDbContext<YggdrasilDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));

        // AddIdentityCore, NOT AddIdentity: AddIdentity also registers cookie
        // authentication and overwrites DefaultAuthenticateScheme, which turns every
        // protected endpoint into a 302 to /Account/Login instead of a 401.
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                // Defaults to false, and login looks users up by email.
                options.User.RequireUniqueEmail = true;

                // RegisterRequestValidator is the authority on the password policy;
                // these are relaxed so Identity never produces a competing message.
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<YggdrasilDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserLookupService, IdentityService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IQuizService, QuizService>();

        return services;
    }
}
