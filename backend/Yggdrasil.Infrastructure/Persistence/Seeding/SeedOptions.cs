namespace Yggdrasil.Infrastructure.Persistence.Seeding;

public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public string Password { get; init; } = string.Empty;
}
