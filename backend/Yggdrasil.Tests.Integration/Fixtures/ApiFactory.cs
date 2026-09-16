using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

using Yggdrasil.Application.Options;
using Yggdrasil.Infrastructure.Persistence;

namespace Yggdrasil.Tests.Integration.Fixtures;

/// <summary>
/// The API running against a throwaway PostgreSQL
/// <see cref="ApiCollection"/> so the assembly starts one container rather than one per class.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string SigningKey = "integration-tests-signing-key-not-a-secret-0123456789";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(
        "postgres:17-alpine"
    ).Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Postgres", _container.GetConnectionString());
        builder.UseSetting(
            $"{JwtOptions.SectionName}:{nameof(JwtOptions.IssuerSigningKey)}",
            SigningKey
        );

        builder.ConfigureTestServices(services =>
            services.AddSingleton<IStartupFilter, ProtectedProbe>()
        );
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<YggdrasilDbContext>()
            .Database.MigrateAsync();
    }

    /// <summary>
    /// Puts the database back to a known state between tests
    /// </summary>
    public async Task ResetAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<YggdrasilDbContext>()
            .Database.ExecuteSqlRawAsync("""TRUNCATE "AspNetUsers" CASCADE""");
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _container.DisposeAsync();
        await base.DisposeAsync();
    }
}
