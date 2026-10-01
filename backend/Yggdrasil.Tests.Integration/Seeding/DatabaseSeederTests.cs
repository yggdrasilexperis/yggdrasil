using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shouldly;

using Testcontainers.PostgreSql;

using Yggdrasil.Infrastructure.Identity;
using Yggdrasil.Infrastructure.Persistence;
using Yggdrasil.Infrastructure.Persistence.Seeding;

namespace Yggdrasil.Tests.Integration.Seeding;

public sealed class SeededDatabase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(
        "postgres:17-alpine"
    ).Build();

    public YggdrasilDbContext Db { get; private set; } = null!;

    public const string SeedPassword = "Integration123!";

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Db = NewContext();
        await Db.Database.MigrateAsync();

        // want to check that seeding isnt duplicated
        var options = Options.Create(new SeedOptions { Password = SeedPassword });
        await new DatabaseSeeder(NewContext(), options).SeedAsync();
        await new DatabaseSeeder(NewContext(), options).SeedAsync();
    }

    public YggdrasilDbContext NewContext() =>
        new(
            new DbContextOptionsBuilder<YggdrasilDbContext>()
                .UseNpgsql(_container.GetConnectionString())
                .Options
        );

    public async Task DisposeAsync()
    {
        await Db.DisposeAsync();
        await _container.DisposeAsync();
    }
}

public class DatabaseSeederTests(SeededDatabase fixture) : IClassFixture<SeededDatabase>
{
    private YggdrasilDbContext Db => fixture.Db;

    [Fact]
    public async Task RefusesToSeedWithoutAPassword()
    {
        var seeder = new DatabaseSeeder(fixture.NewContext(), Options.Create(new SeedOptions()));

        await Should.ThrowAsync<InvalidOperationException>(() => seeder.SeedAsync());
    }

    [Fact]
    public async Task SeedsEveryEntity_AndSeedingTwiceDoesNotDuplicate()
    {
        (await Db.Users.CountAsync()).ShouldBe(8);
        (await Db.Roles.CountAsync()).ShouldBe(2);
        (await Db.UserRoles.CountAsync()).ShouldBe(8);
        (await Db.Categories.CountAsync()).ShouldBe(13);
        (await Db.Quizzes.CountAsync()).ShouldBe(46);
        (await Db.Questions.CountAsync()).ShouldBe(338);
        (await Db.AnswerOptions.CountAsync()).ShouldBe(1310);
        (await Db.Comments.CountAsync()).ShouldBe(144);
    }

    [Fact]
    public async Task EveryQuizIsTaggedWithAtLeastOneCategory()
    {
        var quizzes = await Db.Quizzes.Include(q => q.Categories).ToListAsync();
        quizzes.ShouldAllBe(q => q.Categories.Count >= 1);
        quizzes
            .SelectMany(q => q.Categories)
            .Select(c => c.Slug)
            .Distinct()
            .Order()
            .ShouldBe(
                [
                    "books", "food-and-drink", "games", "geography", "history", "movies", "music",
                    "nature", "pop-culture", "science", "sports", "tv-shows", "uncategorized",
                ]
            );
    }

    [Fact]
    public async Task SomeQuizzesCarrySeveralCategories()
    {
        var quizzes = await Db.Quizzes.Include(q => q.Categories).ToListAsync();

        quizzes.Count(q => q.Categories.Count == 2).ShouldBe(10);
        quizzes.Count(q => q.Categories.Count == 3).ShouldBe(1);
        // 35 quizzes with one category, 10 with two, 1 with three
        quizzes.SelectMany(q => q.Categories).Count().ShouldBe(58);
    }

    [Fact]
    public async Task OneCategoryIsSharedAcrossQuizzes_WithoutDuplicatingTheCategoryRow()
    {
        var popCulture = await Db
            .Categories.Include(c => c.Quizzes)
            .SingleAsync(c => c.Slug == "pop-culture");

        popCulture.Quizzes.Count.ShouldBe(6);
        (await Db.Categories.CountAsync(c => c.Slug == "pop-culture")).ShouldBe(1);
    }

    [Fact]
    public async Task EveryQuestionHasAtLeastTwoOptionsAndExactlyOneCorrectAnswer()
    {
        var questions = await Db.Questions.Include(q => q.AnswerOptions).ToListAsync();
        questions.Count.ShouldBe(338);
        // Mostly four options, plus some true/false and three-way questions.
        questions.ShouldAllBe(q => q.AnswerOptions.Count >= 2 && q.AnswerOptions.Count <= 4);
        questions.ShouldContain(q => q.AnswerOptions.Count == 2);
        questions.ShouldAllBe(q => q.AnswerOptions.Count(o => o.IsCorrect) == 1);
    }

    [Fact]
    public async Task QuizzesAreSpreadAcrossSeveralOwners_SoOwnershipRulesAreDemonstrable()
    {
        var alva = await Db.Users.SingleAsync(u => u.NormalizedEmail == "ALVA@EXAMPLE.COM");
        var jonas = await Db.Users.SingleAsync(u => u.NormalizedEmail == "JONAS@EXAMPLE.COM");
        var admin = await Db.Users.SingleAsync(u => u.NormalizedEmail == "ADMIN@EXAMPLE.COM");

        var owners = await Db.Quizzes.Select(q => q.OwnerId).Distinct().ToListAsync();
        owners.Count.ShouldBe(7);
        owners.ShouldContain(alva.Id);
        owners.ShouldContain(jonas.Id);
        // The admin owns nothing, so anything they change is someone else's quiz.
        owners.ShouldNotContain(admin.Id);
    }

    [Fact]
    public async Task CommentsComeFromOtherUsers_WithSomeRepliesFromTheQuizAuthor()
    {
        var pairs = await Db
            .Comments.Join(
                Db.Quizzes,
                c => c.QuizId,
                q => q.Id,
                (c, q) => new { c.AuthorId, q.OwnerId }
            )
            .ToListAsync();
        pairs.Count(x => x.AuthorId != x.OwnerId).ShouldBeGreaterThan(pairs.Count(x => x.AuthorId == x.OwnerId));
        pairs.ShouldContain(x => x.AuthorId == x.OwnerId);
    }

    [Fact]
    public async Task CommentsArePostedAfterTheirQuiz()
    {
        var comments = await Db
            .Comments.Join(
                Db.Quizzes,
                c => c.QuizId,
                q => q.Id,
                (c, q) => new { c.CreatedAt, c.UpdatedAt, QuizCreatedAt = q.CreatedAt }
            )
            .ToListAsync();
        comments.ShouldAllBe(x => x.CreatedAt > x.QuizCreatedAt);
        comments.ShouldAllBe(x => x.UpdatedAt >= x.CreatedAt);
    }

    [Fact]
    public async Task SeededUsersCanAuthenticateWithTheDocumentedPassword()
    {
        var alva = await Db.Users.SingleAsync(u => u.NormalizedEmail == "ALVA@EXAMPLE.COM");
        alva.NormalizedUserName.ShouldBe("ALVA");
        alva.SecurityStamp.ShouldNotBeNullOrEmpty();

        new PasswordHasher<ApplicationUser>()
            .VerifyHashedPassword(alva, alva.PasswordHash!, SeededDatabase.SeedPassword)
            .ShouldBe(PasswordVerificationResult.Success);
    }

    [Fact]
    public async Task TimestampsRoundTripAsUtc()
    {
        var quiz = await Db.Quizzes.OrderBy(q => q.CreatedAt).FirstAsync();
        quiz.CreatedAt.Offset.ShouldBe(TimeSpan.Zero);
        quiz.UpdatedAt.ShouldNotBe(default);
    }
}
