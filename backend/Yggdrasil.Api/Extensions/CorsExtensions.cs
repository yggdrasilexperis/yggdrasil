namespace Yggdrasil.Api.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "Frontend";
    private const string OriginsPath = "Cors:AllowedOrigins";

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection(OriginsPath).Get<string[]>() ?? [];

        Validate(origins);

        // This is a temp solution, stricter cors policy can be configured later
        services.AddCors(options =>
        {
            options.AddPolicy(
                PolicyName,
                policy => policy
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        return services;
    }

    // Private helper for validating config
    private static void Validate(string[] origins)
    {
        if (origins.Length == 0)
        {
            throw new InvalidOperationException(
                $"{OriginsPath} is empty. The browser blocks every request from the "
                + "frontend without it. Add the origins as a JSON array, for example:\n"
                + "  \"Cors\": { \"AllowedOrigins\": [ \"http://localhost:5173\" ] }"
            );
        }

        foreach (var origin in origins)
        {
            // WithOrigins compares the string against the browser's Origin header,
            // which never carries a path or a trailing slash. "http://localhost:5173/"
            // therefore matches nothing, silently.
            if (
                !Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                || uri.AbsolutePath != "/"
                || origin.EndsWith('/')
            )
            {
                throw new InvalidOperationException(
                    $"{OriginsPath} contains '{origin}', which is not a bare origin. "
                    + "Expected scheme, host and port with no trailing slash, "
                    + "for example 'http://localhost:5173'."
                );
            }
        }
    }
}
