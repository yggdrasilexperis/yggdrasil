using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Db = NewContext();
        await Db.Database.MigrateAsync();

        // want to check that seeding isnt duplicated
        await new DatabaseSeeder(NewContext()).SeedAsync();
        await new DatabaseSeeder(NewContext()).SeedAsync();
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
    public async Task SeedsEveryEntity_AndSeedingTwiceDoesNotDuplicate()
    {
        (await Db.Users.CountAsync()).ShouldBe(3);
        (await Db.Roles.CountAsync()).ShouldBe(2);
        (await Db.UserRoles.CountAsync()).ShouldBe(3);
        (await Db.Categories.CountAsync()).ShouldBe(5);
        (await Db.Quizzes.CountAsync()).ShouldBe(4);
        (await Db.Questions.CountAsync()).ShouldBe(16);
        (await Db.AnswerOptions.CountAsync()).ShouldBe(64);
        (await Db.Comments.CountAsync()).ShouldBe(3);
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
            .ShouldBe(["games", "music", "pop-culture", "sports", "tv-shows"]);
    }

    [Fact]
    public async Task TwoQuizzesCarryTwoCategoriesEach()
    {
        var quizzes = await Db.Quizzes.Include(q => q.Categories).ToListAsync();

        quizzes.Count(q => q.Categories.Count == 2).ShouldBe(2);
        quizzes.SelectMany(q => q.Categories).Count().ShouldBe(6);
    }

    [Fact]
    public async Task OneCategoryIsSharedAcrossQuizzes_WithoutDuplicatingTheCategoryRow()
    {
        var popCulture = await Db
            .Categories.Include(c => c.Quizzes)
            .SingleAsync(c => c.Slug == "pop-culture");

        popCulture.Quizzes.Count.ShouldBe(2);
        (await Db.Categories.CountAsync(c => c.Slug == "pop-culture")).ShouldBe(1);
    }

    [Fact]
    public async Task EveryQuestionHasFourOptionsAndExactlyOneCorrectAnswer()
    {
        var questions = await Db.Questions.Include(q => q.AnswerOptions).ToListAsync();
        questions.Count.ShouldBe(16);
        questions.ShouldAllBe(q => q.AnswerOptions.Count == 4);
        questions.ShouldAllBe(q => q.AnswerOptions.Count(o => o.IsCorrect) == 1);
    }

    [Fact]
    public async Task QuizzesAreSplitAcrossTwoOwners_SoOwnershipRulesAreDemonstrable()
    {
        var alva = await Db.Users.SingleAsync(u => u.NormalizedEmail == "ALVA@EXAMPLE.COM");
        var jonas = await Db.Users.SingleAsync(u => u.NormalizedEmail == "JONAS@EXAMPLE.COM");

        var owners = await Db.Quizzes.Select(q => q.OwnerId).Distinct().ToListAsync();
        owners.Count.ShouldBe(2);
        owners.ShouldContain(alva.Id);
        owners.ShouldContain(jonas.Id);
    }

    [Fact]
    public async Task CommentsAreWrittenByUsersWhoDoNotOwnTheQuiz()
    {
        var pairs = await Db
            .Comments.Join(
                Db.Quizzes,
                c => c.QuizId,
                q => q.Id,
                (c, q) => new { c.AuthorId, q.OwnerId }
            )
            .ToListAsync();
        pairs.ShouldAllBe(x => x.AuthorId != x.OwnerId);
    }

    [Fact]
    public async Task SeededUsersCanAuthenticateWithTheDocumentedPassword()
    {
        var alva = await Db.Users.SingleAsync(u => u.NormalizedEmail == "ALVA@EXAMPLE.COM");
        alva.NormalizedUserName.ShouldBe("ALVA");
        alva.SecurityStamp.ShouldNotBeNullOrEmpty();

        new PasswordHasher<ApplicationUser>()
            .VerifyHashedPassword(alva, alva.PasswordHash!, "Password123!")
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
