using Yggdrasil.Api.Endpoints;
using Yggdrasil.Api.Extensions;
using Yggdrasil.Api.Handlers;
using Yggdrasil.Application;
using Yggdrasil.Application.Abstractions;
using Yggdrasil.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddJwtAuth(builder.Configuration, builder.Environment);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
builder.Services.AddCurrentUser();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddApiDocumentation();

var app = builder.Build();

if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>().SeedAsync();
    return;
}

app.UseCors(CorsExtensions.PolicyName);
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapApiDocumentation();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapQuizEndpoints();
app.MapCategoryEndpoints();

app.MapFallback("api/{*path}", () => TypedResults.NotFound());
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { } // needed for integration tests down the road
