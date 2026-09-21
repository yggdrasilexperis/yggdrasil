using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Domain.Enums;
using Yggdrasil.Infrastructure.Persistence;
using Yggdrasil.Tests.Integration.Fixtures;

namespace Yggdrasil.Tests.Integration.Quiz;

[Collection(ApiCollection.Name)]
public sealed class CreateAndDeleteQuizEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private const string Url = "api/quizzes";

    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly HttpClient _client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();

        // CreateQuizRequestValidator requires at least one category, and ResetAsync truncates them.
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<YggdrasilDbContext>();
        db.Categories.Add(new Category
        {
            Id = _categoryId,
            Name = "Music",
            Slug = "music",
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateQuiz_Returns201WithALocationThatResolvesToTheQuiz()
    {
        await SignInAsync();

        var response = await _client.PostAsJsonAsync(Url, NewQuiz());

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var quiz = (await response.Content.ReadFromJsonAsync<QuizResponse>())!;
        response.Headers.Location!.ToString().ShouldBe($"/api/quizzes/{quiz.Id}");

        (await _client.GetAsync(response.Headers.Location)).StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateQuiz_WithoutAToken_Returns401()
    {
        var response = await _client.PostAsJsonAsync(Url, NewQuiz());

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteQuiz_AsTheOwner_Returns204AndTheQuizIsGone()
    {
        await SignInAsync();
        var created = await _client.PostAsJsonAsync(Url, NewQuiz());
        var location = created.Headers.Location!;

        var response = await _client.DeleteAsync(location);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await _client.GetAsync(location)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private async Task SignInAsync()
    {
        var registration = await _client.PostAsJsonAsync(
            "api/auth/register",
            new RegisterRequest("ada@example.com", "ada_lovelace", "correcthorse")
        );
        var account = (await registration.Content.ReadFromJsonAsync<AuthResponse>())!;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", account.Token);
    }

    private CreateQuizRequest NewQuiz() =>
        new("Capitals", "Name the capital", Difficulty.Normal, [_categoryId]);
}