using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Domain.Constants;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Domain.Enums;
using Yggdrasil.Infrastructure.Persistence;
using Yggdrasil.Tests.Integration.Fixtures;

namespace Yggdrasil.Tests.Integration.Quiz;

[Collection(ApiCollection.Name)]
public sealed class UpdateQuizCategoriesEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private const string Url = "api/quizzes";

    private readonly Guid _musicId = Guid.NewGuid();
    private readonly Guid _sportsId = Guid.NewGuid();
    private readonly HttpClient _client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<YggdrasilDbContext>();
        db.Categories.AddRange(
            new Category { Id = _musicId, Name = "Music", Slug = "music", CreatedAt = DateTimeOffset.UtcNow },
            new Category { Id = _sportsId, Name = "Sports", Slug = "sports", CreatedAt = DateTimeOffset.UtcNow },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Uncategorized",
                Slug = CategorySlugs.Uncategorized,
                CreatedAt = DateTimeOffset.UtcNow,
            }
        );
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task UpdateQuiz_WithNewCategoryIds_ReplacesTheTagsWithoutCreatingCategories()
    {
        await SignInAsync();
        var quiz = await CreateQuizAsync(_musicId);

        var response = await _client.PutAsJsonAsync($"{Url}/{quiz.Id}", UpdateWith(_sportsId));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updated = (await response.Content.ReadFromJsonAsync<QuizResponse>())!;
        updated.Categories.ShouldHaveSingleItem().Slug.ShouldBe("sports");

        (await GetPageAsync("music")).TotalCount.ShouldBe(0);
        (await GetPageAsync("sports")).Items.ShouldHaveSingleItem().Id.ShouldBe(quiz.Id);

        // Still only the three categories seeded above: re-tagging reuses them.
        var categories = await _client.GetFromJsonAsync<List<CategoryResponse>>("api/categories");
        categories!.Count.ShouldBe(3);
    }

    [Fact]
    public async Task UpdateQuiz_WithNoCategoryIds_FallsBackToUncategorized()
    {
        await SignInAsync();
        var quiz = await CreateQuizAsync(_musicId);

        var response = await _client.PutAsJsonAsync($"{Url}/{quiz.Id}", UpdateWith());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updated = (await response.Content.ReadFromJsonAsync<QuizResponse>())!;
        updated.Categories.ShouldHaveSingleItem().Slug.ShouldBe(CategorySlugs.Uncategorized);
    }

    private async Task<QuizResponse> CreateQuizAsync(params Guid[] categoryIds)
    {
        var response = await _client.PostAsJsonAsync(
            Url,
            new CreateQuizRequest("Capitals", "Name the capital", Difficulty.Normal, categoryIds)
        );
        return (await response.Content.ReadFromJsonAsync<QuizResponse>())!;
    }

    private async Task<PagedResult<QuizResponse>> GetPageAsync(string categorySlug) =>
        (await _client.GetFromJsonAsync<PagedResult<QuizResponse>>($"{Url}?categorySlugs={categorySlug}"))!;

    private static UpdateQuizRequest UpdateWith(params Guid[] categoryIds) =>
        new("Capitals", "Name the capital", Difficulty.Normal, categoryIds);

    private async Task SignInAsync()
    {
        var registration = await _client.PostAsJsonAsync(
            "api/auth/register",
            new RegisterRequest("ada@example.com", "ada_lovelace", "correcthorse")
        );
        var account = (await registration.Content.ReadFromJsonAsync<AuthResponse>())!;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", account.Token);
    }
}
