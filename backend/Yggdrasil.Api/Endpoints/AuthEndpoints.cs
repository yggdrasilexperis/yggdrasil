using Microsoft.AspNetCore.Http.HttpResults;

using Yggdrasil.Api.Filters;
using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts.Authentication;

namespace Yggdrasil.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth").WithTags("Authentication").AllowAnonymous();

        group.MapPost("/register", Register).AddEndpointFilter<ValidationFilter<RegisterRequest>>();
        group.MapPost("/login", Login).AddEndpointFilter<ValidationFilter<LoginRequest>>();

        return app;
    }

    private static async Task<Created<AuthResponse>> Register(
        RegisterRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken);
        return TypedResults.Created((string?)null, response);
    }

    private static async Task<Ok<AuthResponse>> Login(
        LoginRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        return TypedResults.Ok(response);
    }
}
