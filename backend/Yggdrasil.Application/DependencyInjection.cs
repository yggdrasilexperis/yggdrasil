using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Services;

namespace Yggdrasil.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
