using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Domain.Enums;
using Yggdrasil.Infrastructure.Identity;
using Yggdrasil.Infrastructure.Persistence;
using Yggdrasil.Tests.Integration.Fixtures;

namespace Yggdrasil.Tests.Integration.Quiz;

[Collection(ApiCollection.Name)]
public sealed class GetQuizzesEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private const string Url = "api/quizzes";

    // Pinned here rather than read from GetQuizzesRequest: this asserts the default the API
    // promises, so changing the record's default should fail this test, not silently follow it.
    private const int DefaultPageSize = 10;

    private static readonly DateTimeOffset BaseDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly Guid OwnerId = Guid.NewGuid();

    private readonly HttpClient _client = factory.CreateClient();

    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetQuizzes_WithNoQueryString_ReturnsFirstPageAtTheDefaultSizeWithTotalCount()
    {
        await SeedQuizzesAsync(Enumerable.Range(1, 15)
            .Select(i => ($"Quiz {i:00}", BaseDate.AddDays(i), (Category[])[]))
            .ToArray());

        var response = await _client.GetAsync(Url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var page = await response.Content.ReadFromJsonAsync<PagedResult<QuizResponse>>();
        page.ShouldNotBeNull();
        page.Page.ShouldBe(1);
        page.PageSize.ShouldBe(DefaultPageSize);
        page.TotalCount.ShouldBe(15);
        page.Items.Count().ShouldBe(DefaultPageSize);
    }

    [Fact]
    public async Task GetQuizzes_WithPageAndPageSize_ReturnsTheRequestedSlice()
    {
        await SeedQuizzesAsync(Enumerable.Range(1, 15)
            .Select(i => ($"Quiz {i:00}", BaseDate.AddDays(i), (Category[])[]))
            .ToArray());

        var response = await _client.GetAsync($"{Url}?page=2&pageSize=5");
        var page = await response.Content.ReadFromJsonAsync<PagedResult<QuizResponse>>();

        page!.Page.ShouldBe(2);
        page.TotalCount.ShouldBe(15);
        page.Items.Count().ShouldBe(5);
        // newest first by default, so page 2 skips the 5 most recently created
        page.Items.Select(q => q.Title).ShouldBe(["Quiz 10", "Quiz 09", "Quiz 08", "Quiz 07", "Quiz 06"]);
    }

    [Theory]
    [InlineData("Title", "Ascending", new[] { "Alpha", "Bravo", "Charlie" })]
    [InlineData("Title", "Descending", new[] { "Charlie", "Bravo", "Alpha" })]
    [InlineData("CreatedAt", "Ascending", new[] { "Charlie", "Alpha", "Bravo" })]
    [InlineData("CreatedAt", "Descending", new[] { "Bravo", "Alpha", "Charlie" })]
    public async Task GetQuizzes_WithSortByAndDirection_OrdersInThatDirection(
    string sortBy, string direction, string[] expected)
    {
        await SeedQuizzesAsync(
            ("Charlie", BaseDate, []),
            ("Alpha", BaseDate.AddDays(1), []),
            ("Bravo", BaseDate.AddDays(2), []));

        var response = await _client.GetAsync($"{Url}?sortBy={sortBy}&sortDirection={direction}");
        var page = await response.Content.ReadFromJsonAsync<PagedResult<QuizResponse>>();

        page!.Items.Select(q => q.Title).ShouldBe(expected);
    }

    [Fact]
    public async Task GetQuizzes_FilteredByCategorySlug_CombinesCorrectlyWithPaging()
    {
        var music = new Category { Id = Guid.NewGuid(), Name = "Music", Slug = "music", CreatedAt = BaseDate };
        var sports = new Category { Id = Guid.NewGuid(), Name = "Sports", Slug = "sports", CreatedAt = BaseDate };

        await SeedQuizzesAsync(
            ("Music 1", BaseDate, [music]),
            ("Music 2", BaseDate.AddDays(1), [music]),
            ("Music 3", BaseDate.AddDays(2), [music]),
            ("Sports 1", BaseDate.AddDays(3), [sports]));

        var response = await _client.GetAsync($"{Url}?categorySlugs=music&pageSize=2");
        var page = await response.Content.ReadFromJsonAsync<PagedResult<QuizResponse>>();

        page!.TotalCount.ShouldBe(3); // the Sports quiz is excluded from the count too
        page.Items.Count().ShouldBe(2);
        page.Items.ShouldAllBe(q => q.Categories.Any(c => c.Slug == "music"));
    }

    [Fact]
    public async Task GetQuizzes_FilteredByTwoCategories_ReturnsOnlyQuizzesCarryingBoth()
    {
        var music = new Category { Id = Guid.NewGuid(), Name = "Music", Slug = "music", CreatedAt = BaseDate };
        var sports = new Category { Id = Guid.NewGuid(), Name = "Sports", Slug = "sports", CreatedAt = BaseDate };

        await SeedQuizzesAsync(
            ("Both", BaseDate, [music, sports]),
            ("Music only", BaseDate.AddDays(1), [music]),
            ("Sports only", BaseDate.AddDays(2), [sports]));

        var response = await _client.GetAsync($"{Url}?categorySlugs=music&categorySlugs=sports");
        var page = await response.Content.ReadFromJsonAsync<PagedResult<QuizResponse>>();

        page!.TotalCount.ShouldBe(1);
        page.Items.ShouldHaveSingleItem().Title.ShouldBe("Both");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(GetQuizzesRequest.MaxPage + 1)]
    [InlineData(int.MaxValue)]
    public async Task GetQuizzes_WhenPageIsOutOfRange_Returns400(int invalidPage)
    {
        var response = await _client.GetAsync($"{Url}?page={invalidPage}");
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(1000)]
    public async Task GetQuizzes_WhenPageSizeIsOutOfRange_Returns400(int invalidPageSize)
    {
        var response = await _client.GetAsync($"{Url}?pageSize={invalidPageSize}");
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private async Task SeedQuizzesAsync(params (string Title, DateTimeOffset CreatedAt, Category[] Categories)[] quizzes)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<YggdrasilDbContext>();

        db.Users.Add(new ApplicationUser
        {
            Id = OwnerId,
            UserName = "owner",
            NormalizedUserName = "OWNER",
            Email = "owner@example.com",
            NormalizedEmail = "OWNER@EXAMPLE.COM",
            // Required by ApplicationUserConfiguration. Never verified — these are anonymous
            // GETs, and the owner only exists to satisfy the quiz's foreign key.
            PasswordHash = "not-a-real-hash",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        });

        foreach (var (title, createdAt, categories) in quizzes)
        {
            // Qualified: this file's namespace ends in .Quiz, which shadows the entity type.
            db.Quizzes.Add(new Domain.Entities.Quiz
            {
                Id = Guid.NewGuid(),
                Title = title,
                Difficulty = Difficulty.Normal,
                OwnerId = OwnerId,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                Categories = categories,
            });
        }

        await db.SaveChangesAsync();
    }
}
