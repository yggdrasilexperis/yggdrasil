using System.Globalization;

using Yggdrasil.Domain.Constants;
using Yggdrasil.Domain.Entities;

namespace Yggdrasil.Infrastructure.Persistence.Seeding;

internal static class SeedData
{
    public static readonly DateTimeOffset At = new(2026, 1, 5, 8, 0, 0, TimeSpan.Zero);

    public static readonly Guid AlvaId = new("0a1b7f2c-0000-4000-8000-000000000001");
    public static readonly Guid JonasId = new("0a1b7f2c-0000-4000-8000-000000000002");
    public static readonly Guid AdminId = new("0a1b7f2c-0000-4000-8000-000000000003");
    public static readonly Guid SigridId = new("0a1b7f2c-0000-4000-8000-000000000004");
    public static readonly Guid TobbenId = new("0a1b7f2c-0000-4000-8000-000000000005");
    public static readonly Guid NoraId = new("0a1b7f2c-0000-4000-8000-000000000006");
    public static readonly Guid KasperId = new("0a1b7f2c-0000-4000-8000-000000000007");
    public static readonly Guid EirikId = new("0a1b7f2c-0000-4000-8000-000000000008");

    public static readonly Guid AdminRoleId = new("0a1b7f2c-0000-4000-8000-0000000000a1");
    public static readonly Guid UserRoleId = new("0a1b7f2c-0000-4000-8000-0000000000a2");

    public static readonly Guid TvShowsId = new("0a1b7f2c-0000-4000-8000-000000000101");
    public static readonly Guid MusicId = new("0a1b7f2c-0000-4000-8000-000000000102");
    public static readonly Guid GamesId = new("0a1b7f2c-0000-4000-8000-000000000103");
    public static readonly Guid SportsId = new("0a1b7f2c-0000-4000-8000-000000000104");
    public static readonly Guid PopCultureId = new("0a1b7f2c-0000-4000-8000-000000000105");
    public static readonly Guid UncategorizedId = new("0a1b7f2c-0000-4000-8000-000000000106");
    public static readonly Guid MoviesId = new("0a1b7f2c-0000-4000-8000-000000000107");
    public static readonly Guid GeographyId = new("0a1b7f2c-0000-4000-8000-000000000108");
    public static readonly Guid HistoryId = new("0a1b7f2c-0000-4000-8000-000000000109");
    public static readonly Guid ScienceId = new("0a1b7f2c-0000-4000-8000-000000000110");
    public static readonly Guid FoodAndDrinkId = new("0a1b7f2c-0000-4000-8000-000000000111");
    public static readonly Guid BooksId = new("0a1b7f2c-0000-4000-8000-000000000112");
    public static readonly Guid NatureId = new("0a1b7f2c-0000-4000-8000-000000000113");

    // Everyone signs in with <username>@example.com and the Seed:Password secret.
    public static readonly IReadOnlyList<(Guid Id, string UserName, Guid RoleId)> Users =
    [
        (AlvaId, "alva", UserRoleId),
        (JonasId, "jonas", UserRoleId),
        (AdminId, "admin", AdminRoleId),
        (SigridId, "sigrid", UserRoleId),
        (TobbenId, "tobben", UserRoleId),
        (NoraId, "nora", UserRoleId),
        (KasperId, "kasper", UserRoleId),
        (EirikId, "eirik", UserRoleId),
    ];

    public static IReadOnlyList<Category> Categories() =>
    [
        new() { Id = TvShowsId, Name = "TV Shows", Slug = "tv-shows", CreatedAt = At },
        new() { Id = MusicId, Name = "Music", Slug = "music", CreatedAt = At },
        new() { Id = GamesId, Name = "Games", Slug = "games", CreatedAt = At },
        new() { Id = SportsId, Name = "Sports", Slug = "sports", CreatedAt = At },
        new() { Id = PopCultureId, Name = "Pop Culture", Slug = "pop-culture", CreatedAt = At },
        new() { Id = UncategorizedId, Name = "Uncategorized", Slug = CategorySlugs.Uncategorized, CreatedAt = At },
        new() { Id = MoviesId, Name = "Movies", Slug = "movies", CreatedAt = At },
        new() { Id = GeographyId, Name = "Geography", Slug = "geography", CreatedAt = At },
        new() { Id = HistoryId, Name = "History", Slug = "history", CreatedAt = At },
        new() { Id = ScienceId, Name = "Science", Slug = "science", CreatedAt = At },
        new() { Id = FoodAndDrinkId, Name = "Food & Drink", Slug = "food-and-drink", CreatedAt = At },
        new() { Id = BooksId, Name = "Books", Slug = "books", CreatedAt = At },
        new() { Id = NatureId, Name = "Nature", Slug = "nature", CreatedAt = At },
    ];

    /// <summary>A UTC timestamp written as "yyyy-MM-dd HH:mm", so seed dates read like dates.</summary>
    public static DateTimeOffset On(string utc) =>
        DateTimeOffset.ParseExact(
            utc,
            "yyyy-MM-dd HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
        );
}
