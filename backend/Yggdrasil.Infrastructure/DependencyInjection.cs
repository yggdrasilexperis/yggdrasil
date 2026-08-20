using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}