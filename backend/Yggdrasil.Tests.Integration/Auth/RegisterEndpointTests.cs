using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using Shouldly;

using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Tests.Integration.Fixtures;

namespace Yggdrasil.Tests.Integration.Auth;

[Collection(ApiCollection.Name)]
public sealed class RegisterEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private const string Url = "api/auth/register";

    private readonly HttpClient _client = factory.CreateClient();

    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_WhenRequestIsValid_Returns201WithTokenAndUser()
    {
        var response = await _client.PostAsJsonAsync(Url, Request());

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body.ShouldNotBeNull();
        body.Token.ShouldNotBeNullOrWhiteSpace();
        body.ExpiresAt.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
        body.User.Id.ShouldNotBe(Guid.Empty);
        body.User.Email.ShouldBe("ada@example.com");
        body.User.UserName.ShouldBe("ada_lovelace");
    }

    [Fact]
    public async Task Register_WhenRequestIsValid_AllowsTheUserToLogIn()
    {
        await _client.PostAsJsonAsync(Url, Request());

        var login = await _client.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest("ada@example.com", "correcthorse")
        );

        login.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ResponseNeverContainsAPasswordHash()
    {
        var response = await _client.PostAsJsonAsync(Url, Request());

        var raw = await response.Content.ReadAsStringAsync();
        raw.ShouldNotContain("hash", Case.Insensitive);
        raw.ShouldNotContain("correcthorse");
    }

    [Fact]
    public async Task Register_WhenEmailIsAlreadyRegistered_Returns409()
    {
        await _client.PostAsJsonAsync(Url, Request());

        var response = await _client.PostAsJsonAsync(Url, Request(userName: "ada_second"));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        // GlobalExceptionHandler writes AppException.Title and .Message, never .Code, so the
        // detail is the only thing telling the two conflicts apart on the wire.
        var problem = await ReadProblem(response);
        problem.Title.ShouldBe("Conflict");
        problem.Detail.ShouldBe("The email address is already registered.");
    }

    [Fact]
    public async Task Register_WhenUserNameIsAlreadyTaken_Returns409()
    {
        await _client.PostAsJsonAsync(Url, Request());

        var response = await _client.PostAsJsonAsync(Url, Request(email: "grace@example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var problem = await ReadProblem(response);
        problem.Title.ShouldBe("Conflict");
        problem.Detail.ShouldBe("The username is already registered.");
    }

    [Fact]
    public async Task Register_WhenPasswordIsTooShort_Returns400WithFieldErrors()
    {
        var response = await _client.PostAsJsonAsync(Url, Request(password: "short"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var problem = await ReadProblem(response);
        problem.Extensions.ShouldContainKey("errors");

        var errors = (JsonElement)problem.Extensions["errors"]!;
        errors
            .TryGetProperty(nameof(RegisterRequest.Password), out _)
            .ShouldBeTrue($"Expected a Password entry, got {errors}");
    }

    [Fact]
    public async Task Register_WhenBodyIsMalformedJson_Returns400()
    {
        var response = await _client.PostAsync(
            Url,
            new StringContent("{ not json", Encoding.UTF8, "application/json")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await ReadProblem(response)).Title.ShouldBe("Invalid request");
    }

    private static RegisterRequest Request(
        string email = "ada@example.com",
        string userName = "ada_lovelace",
        string password = "correcthorse"
    ) => new(email, userName, password);

    private static async Task<ProblemDetails> ReadProblem(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ProblemDetails>())!;
}
