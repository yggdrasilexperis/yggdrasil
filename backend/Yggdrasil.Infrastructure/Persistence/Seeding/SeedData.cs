using Yggdrasil.Domain.Entities;
using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Infrastructure.Persistence.Seeding;

internal static class SeedData
{
    public static readonly DateTimeOffset At = new(2026, 3, 14, 9, 30, 0, TimeSpan.Zero);

    public static readonly Guid AlvaId = new("0a1b7f2c-0000-4000-8000-000000000001");
    public static readonly Guid JonasId = new("0a1b7f2c-0000-4000-8000-000000000002");
    public static readonly Guid AdminId = new("0a1b7f2c-0000-4000-8000-000000000003");

    public static readonly Guid AdminRoleId = new("0a1b7f2c-0000-4000-8000-0000000000a1");
    public static readonly Guid UserRoleId = new("0a1b7f2c-0000-4000-8000-0000000000a2");

    public static readonly Guid TvShowsId = new("0a1b7f2c-0000-4000-8000-000000000101");
    public static readonly Guid MusicId = new("0a1b7f2c-0000-4000-8000-000000000102");
    public static readonly Guid GamesId = new("0a1b7f2c-0000-4000-8000-000000000103");
    public static readonly Guid SportsId = new("0a1b7f2c-0000-4000-8000-000000000104");
    public static readonly Guid PopCultureId = new("0a1b7f2c-0000-4000-8000-000000000105");

    public static IReadOnlyList<Category> Categories() =>
    [
        new() { Id = TvShowsId, Name = "TV Shows", Slug = "tv-shows", CreatedAt = At },
        new() { Id = MusicId,   Name = "Music",    Slug = "music",    CreatedAt = At },
        new() { Id = GamesId,   Name = "Games",    Slug = "games",    CreatedAt = At },
        new() { Id = SportsId,  Name = "Sports",   Slug = "sports",   CreatedAt = At },
        new() { Id = PopCultureId, Name = "Pop Culture", Slug = "pop-culture", CreatedAt = At },
    ];

    public static IEnumerable<Quiz> Quizzes(IReadOnlyList<Category> categories)
    {
        Category Of(Guid id) => categories.Single(c => c.Id == id);

        yield return GameOfThrones(Of(TvShowsId), Of(PopCultureId));
        yield return Rap(Of(MusicId), Of(PopCultureId));
        yield return FirstPersonShooters(Of(GamesId));
        yield return ChampionsLeague2025(Of(SportsId));
    }

    private static Quiz GameOfThrones(params Category[] categories) =>
        new()
        {
            Id = new("0a1b7f2c-0000-4000-8000-000000000201"),
            Title = "Game of Thrones",
            Description =
                "Eight seasons of Westeros, from the Winterfell crypts to the "
                + "Iron Throne. Book readers get no advantage here.",
            Difficulty = Difficulty.Normal,
            OwnerId = AlvaId,
            CreatedAt = At,
            UpdatedAt = At,
            Categories = categories,
            Questions =
            [
                Q("000000000311", "What are the words of House Stark?",
                    O("000000004111", "Winter Is Coming", correct: true),
                    O("000000004112", "Hear Me Roar"),
                    O("000000004113", "Fire and Blood"),
                    O("000000004114", "Ours Is the Fury")),
                Q("000000000312", "Who kills the Night King?",
                    O("000000004121", "Jon Snow"),
                    O("000000004122", "Arya Stark", correct: true),
                    O("000000004123", "Daenerys Targaryen"),
                    O("000000004124", "Theon Greyjoy")),
                Q("000000000313", "Which lord hosts the Red Wedding?",
                    O("000000004131", "Roose Bolton"),
                    O("000000004132", "Tywin Lannister"),
                    O("000000004133", "Walder Frey", correct: true),
                    O("000000004134", "Petyr Baelish")),
                Q("000000000314", "Who is revealed to be Jon Snow's mother?",
                    O("000000004141", "Lyanna Stark", correct: true),
                    O("000000004142", "Catelyn Stark"),
                    O("000000004143", "Ashara Dayne"),
                    O("000000004144", "Cersei Lannister")),
            ],
        };


    private static Quiz Rap(params Category[] categories) =>
        new()
        {
            Id = new("0a1b7f2c-0000-4000-8000-000000000202"),
            Title = "Rap: Four Decades of Records",
            Description =
                "From block parties in the Bronx to the Pulitzer committee. "
                + "Albums, producers, and the records that moved the genre.",
            Difficulty = Difficulty.Hard,
            OwnerId = AlvaId,
            CreatedAt = At.AddDays(3),
            UpdatedAt = At.AddDays(3),
            Categories = categories,
            Questions =
            [
                Q("000000000321", "Which 1994 debut album was Nas's first release?",
                    O("000000004211", "Illmatic", correct: true),
                    O("000000004212", "It Was Written"),
                    O("000000004213", "Ready to Die"),
                    O("000000004214", "Reasonable Doubt")),
                Q("000000000322", "Which Kendrick Lamar album won the Pulitzer Prize for Music?",
                    O("000000004221", "good kid, m.A.A.d city"),
                    O("000000004222", "To Pimp a Butterfly"),
                    O("000000004223", "DAMN.", correct: true),
                    O("000000004224", "Mr. Morale & The Big Steppers")),
                Q("000000000323", "Who produced and released 'The Chronic' in 1992?",
                    O("000000004231", "Dr. Dre", correct: true),
                    O("000000004232", "Ice Cube"),
                    O("000000004233", "DJ Premier"),
                    O("000000004234", "Rick Rubin")),
                Q("000000000324", "What is the title of Wu-Tang Clan's 1993 debut album?",
                    O("000000004241", "Wu-Tang Forever"),
                    O("000000004242", "Enter the Wu-Tang (36 Chambers)", correct: true),
                    O("000000004243", "Liquid Swords"),
                    O("000000004244", "Only Built 4 Cuban Linx...")),
            ],
        };


    private static Quiz FirstPersonShooters(params Category[] categories) =>
        new()
        {
            Id = new("0a1b7f2c-0000-4000-8000-000000000203"),
            Title = "FPS Games: Thirty Years Down the Barrel",
            Description =
                "The studios, engines and releases that built the first-person "
                + "shooter, from shareware floppies to the modern era.",
            Difficulty = Difficulty.Normal,
            OwnerId = JonasId,
            CreatedAt = At.AddDays(6),
            UpdatedAt = At.AddDays(6),
            Categories = categories,
            Questions =
            [
                Q("000000000331", "Which studio released Doom in 1993?",
                    O("000000004311", "id Software", correct: true),
                    O("000000004312", "Valve"),
                    O("000000004313", "Apogee Software"),
                    O("000000004314", "Bungie")),
                Q("000000000332", "Counter-Strike began life as a mod for which game?",
                    O("000000004321", "Quake III Arena"),
                    O("000000004322", "Half-Life", correct: true),
                    O("000000004323", "Unreal Tournament"),
                    O("000000004324", "Team Fortress Classic")),
                Q("000000000333", "Which engine powered Half-Life 2 at its 2004 release?",
                    O("000000004331", "GoldSrc"),
                    O("000000004332", "id Tech 4"),
                    O("000000004333", "Source", correct: true),
                    O("000000004334", "Unreal Engine 2")),
                Q("000000000334", "Which console did GoldenEye 007 launch on in 1997?",
                    O("000000004341", "PlayStation"),
                    O("000000004342", "Sega Saturn"),
                    O("000000004343", "Nintendo 64", correct: true),
                    O("000000004344", "Game Boy Color")),
            ],
        };

    private static Quiz ChampionsLeague2025(params Category[] categories) =>
        new()
        {
            Id = new("0a1b7f2c-0000-4000-8000-000000000204"),
            Title = "Champions League 2025: The Munich Final",
            Description =
                "The 2024–25 campaign — the first run under the 36-team league "
                + "phase — and the final that ended it.",
            Difficulty = Difficulty.Easy,
            OwnerId = JonasId,
            CreatedAt = At.AddDays(9),
            UpdatedAt = At.AddDays(9),
            Categories = categories,
            Questions =
            [
                Q("000000000341", "Which club won the 2024–25 UEFA Champions League?",
                    O("000000004411", "Inter Milan"),
                    O("000000004412", "Paris Saint-Germain", correct: true),
                    O("000000004413", "Real Madrid"),
                    O("000000004414", "Arsenal")),
                Q("000000000342", "What was the score in the 2025 final?",
                    O("000000004421", "5–0", correct: true),
                    O("000000004422", "3–1"),
                    O("000000004423", "2–0"),
                    O("000000004424", "1–0 after extra time")),
                Q("000000000343", "Which city hosted the 2025 final?",
                    O("000000004431", "London"),
                    O("000000004432", "Istanbul"),
                    O("000000004433", "Munich", correct: true),
                    O("000000004434", "Lisbon")),
                Q("000000000344", "Which PSG player scored twice in the final?",
                    O("000000004441", "Ousmane Dembélé"),
                    O("000000004442", "Achraf Hakimi"),
                    O("000000004443", "Désiré Doué", correct: true),
                    O("000000004444", "Khvicha Kvaratskhelia")),
            ],
        };

    public static IEnumerable<Comment> Comments() =>
    [
        new()
        {
            Id = new("0a1b7f2c-0000-4000-8000-000000000501"),
            QuizId = new("0a1b7f2c-0000-4000-8000-000000000201"),
            AuthorId = JonasId,
            Body = "Question four is generous — the show telegraphs that one from season six.",
            CreatedAt = At.AddDays(1),
            UpdatedAt = At.AddDays(1),
        },
        new()
        {
            Id = new("0a1b7f2c-0000-4000-8000-000000000502"),
            QuizId = new("0a1b7f2c-0000-4000-8000-000000000203"),
            AuthorId = AlvaId,
            Body = "Good spread of eras. Would add something on the Quake engine lineage.",
            CreatedAt = At.AddDays(7),
            UpdatedAt = At.AddDays(7),
        },
        new()
        {
            Id = new("0a1b7f2c-0000-4000-8000-000000000503"),
            QuizId = new("0a1b7f2c-0000-4000-8000-000000000204"),
            AuthorId = AdminId,
            Body = "Nice one. Worth a follow-up quiz on the new league phase format.",
            CreatedAt = At.AddDays(10),
            UpdatedAt = At.AddDays(10),
        },
    ];

    private static Question Q(string id, string text, params AnswerOption[] options) =>
        new()
        {
            Id = new($"0a1b7f2c-0000-4000-8000-{id}"),
            Text = text,
            CreatedAt = At,
            AnswerOptions = options,
        };

    private static AnswerOption O(string id, string text, bool correct = false) =>
        new()
        {
            Id = new($"0a1b7f2c-0000-4000-8000-{id}"),
            Text = text,
            IsCorrect = correct,
            CreatedAt = At,
        };
}
