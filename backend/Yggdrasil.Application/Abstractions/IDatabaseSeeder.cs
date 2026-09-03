namespace Yggdrasil.Application.Abstractions;

public interface IDatabaseSeeder
{
    public Task SeedAsync(CancellationToken ct = default);
}
