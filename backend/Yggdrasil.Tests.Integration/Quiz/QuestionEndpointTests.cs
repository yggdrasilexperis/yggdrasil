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
public sealed class QuestionEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private const string Url = "api/quizzes";

    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly HttpClient _client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<YggdrasilDbContext>();
        db.Categories.Add(
            new Category
            {
                Id = _categoryId,
                Name = "Geography",
                Slug = "geography",
                CreatedAt = DateTimeOffset.UtcNow,
            }
        );
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddQuestion_AsTheOwner_Returns201AndTheQuestionAppearsOnTheQuiz()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");
        var quizId = await CreateQuizAsync();

        var response = await _client.PostAsJsonAsync($"{Url}/{quizId}/questions", NewQuestion());

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var question = (await response.Content.ReadFromJsonAsync<QuestionResponse>())!;
        question.Text.ShouldBe("Capital of Norway?");
        question.AnswerOptions.Count().ShouldBe(2);

        var quiz = await _client.GetFromJsonAsync<QuizContentResponse>($"{Url}/{quizId}");
        quiz!.Questions.ShouldHaveSingleItem().Id.ShouldBe(question.Id);
    }

    [Fact]
    public async Task AddQuestion_WhenTheQuizDoesNotExist_Returns404()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");

        var response = await _client.PostAsJsonAsync(
            $"{Url}/{Guid.NewGuid()}/questions",
            NewQuestion()
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddQuestion_WithoutAToken_Returns401()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");
        var quizId = await CreateQuizAsync();
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync($"{Url}/{quizId}/questions", NewQuestion());

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddQuestion_AsSomeoneElse_Returns403()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");
        var quizId = await CreateQuizAsync();
        await SignInAsync("grace@example.com", "grace_hopper");

        var response = await _client.PostAsJsonAsync($"{Url}/{quizId}/questions", NewQuestion());

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddQuestion_WithOnlyOneAnswerOption_Returns400()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");
        var quizId = await CreateQuizAsync();

        var request = new CreateQuestionRequest(
            "Capital of Norway?",
            [new CreateAnswerOptionRequest("Oslo", true)]
        );
        var response = await _client.PostAsJsonAsync($"{Url}/{quizId}/questions", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteQuestion_AsTheOwner_Returns204AndTheQuestionIsGone()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");
        var quizId = await CreateQuizAsync();
        var questionId = await AddQuestionAsync(quizId);

        var response = await _client.DeleteAsync($"{Url}/{quizId}/questions/{questionId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var questions = await _client.GetFromJsonAsync<List<QuestionResponse>>(
            $"{Url}/{quizId}/questions"
        );
        questions.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteQuestion_WhenItBelongsToAnotherQuiz_Returns404()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");
        var quizId = await CreateQuizAsync();
        var otherQuizId = await CreateQuizAsync("Rivers");
        var questionId = await AddQuestionAsync(quizId);

        var response = await _client.DeleteAsync($"{Url}/{otherQuizId}/questions/{questionId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    // A Restrict foreign key would make Postgres reject this delete and the API answer 500,
    // so a 204 here is what proves the cascade from quiz to questions.
    [Fact]
    public async Task DeleteQuiz_WhenItHasQuestions_Returns204()
    {
        await SignInAsync("ada@example.com", "ada_lovelace");
        var quizId = await CreateQuizAsync();
        await AddQuestionAsync(quizId);

        var response = await _client.DeleteAsync($"{Url}/{quizId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await _client.GetAsync($"{Url}/{quizId}")).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private async Task SignInAsync(string email, string username)
    {
        var registration = await _client.PostAsJsonAsync(
            "api/auth/register",
            new RegisterRequest(email, username, "correcthorse")
        );
        var account = (await registration.Content.ReadFromJsonAsync<AuthResponse>())!;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            account.Token
        );
    }

    private async Task<Guid> CreateQuizAsync(string title = "Capitals")
    {
        var request = new CreateQuizRequest(
            title,
            "Name the capital",
            Difficulty.Normal,
            [_categoryId]
        );
        var response = await _client.PostAsJsonAsync(Url, request);

        return (await response.Content.ReadFromJsonAsync<QuizResponse>())!.Id;
    }

    private async Task<Guid> AddQuestionAsync(Guid quizId)
    {
        var response = await _client.PostAsJsonAsync($"{Url}/{quizId}/questions", NewQuestion());

        return (await response.Content.ReadFromJsonAsync<QuestionResponse>())!.Id;
    }

    private static CreateQuestionRequest NewQuestion() =>
        new(
            "Capital of Norway?",
            [
                new CreateAnswerOptionRequest("Oslo", true),
                new CreateAnswerOptionRequest("Bergen", false),
            ]
        );
}
