using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Options;
using Yggdrasil.Infrastructure.Identity;
using Yggdrasil.Infrastructure.Persistence;

namespace Yggdrasil.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<YggdrasilDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres"))
        );

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

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
            .AddEntityFrameworkStores<YggdrasilDbContext>();
        // .AddRoles<IdentityRole<Guid>>() — the one line to add when roles land.

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
