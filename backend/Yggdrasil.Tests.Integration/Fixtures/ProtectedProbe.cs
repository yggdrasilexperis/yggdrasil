using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Yggdrasil.Tests.Integration.Fixtures;

/// <summary>
/// A test only replacement for <c>[Authorize]</c> routes.
/// Should be deleted when real protected endpoints exist. 
/// </summary>
internal sealed class ProtectedProbe : IStartupFilter
{
    public const string Path = "/__test/protected";

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
        app =>
        {
            app.Use(
                async (context, following) =>
                {
                    if (context.Request.Path.Value != Path)
                    {
                        await following();
                        return;
                    }

                    var result = await context.AuthenticateAsync();
                    if (!result.Succeeded)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    await context.Response.WriteAsync(
                        result.Principal!.FindFirst("sub")?.Value ?? string.Empty
                    );
                }
            );

            next(app);
        };
}
