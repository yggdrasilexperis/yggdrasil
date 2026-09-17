using Yggdrasil.Application.Abstractions;
using Yggdrasil.Infrastructure.Identity;

namespace Yggdrasil.Api.Extensions;

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
