using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Infrastructure.Persistence;
using Yggdrasil.Infrastructure.Persistence.Seeding;

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

        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        return services;
    }
}
