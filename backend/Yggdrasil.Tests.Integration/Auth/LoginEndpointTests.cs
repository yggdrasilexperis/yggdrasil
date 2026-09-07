using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

using Shouldly;

using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Tests.Integration.Fixtures;

namespace Yggdrasil.Tests.Integration.Auth;

[Collection(ApiCollection.Name)]
public sealed class LoginEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private const string Url = "api/auth/login";

    private readonly HttpClient _client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        await _client.PostAsJsonAsync(
            "api/auth/register",
            new RegisterRequest("ada@example.com", "ada_lovelace", "correcthorse")
        );
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_WhenCredentialsAreValid_Returns200WithToken()
    {
        var response = await _client.PostAsJsonAsync(Url, Request());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body.ShouldNotBeNull();
        body.Token.ShouldNotBeNullOrWhiteSpace();
        body.ExpiresAt.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
        body.User.Email.ShouldBe("ada@example.com");
    }

    [Fact]
    public async Task Login_WhenEmailDiffersOnlyByCase_Returns200()
    {
        var response = await _client.PostAsJsonAsync(Url, Request(email: "ADA@Example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WhenPasswordIsWrong_Returns401()
    {
        var response = await _client.PostAsJsonAsync(Url, Request(password: "wrongpassword"));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await ReadProblem(response);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Invalid email or password.");
    }

    [Fact]
    public async Task Login_WhenEmailIsUnknown_Returns401()
    {
        var response = await _client.PostAsJsonAsync(Url, Request(email: "nobody@example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await ReadProblem(response);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Invalid email or password.");
    }

    [Fact]
    public async Task Login_WhenEmailIsUnknownOrPasswordIsWrong_ReturnsIdenticalBody()
    {
        var unknownEmail = await _client.PostAsJsonAsync(
            Url,
            Request(email: "nobody@example.com")
        );
        var wrongPassword = await _client.PostAsJsonAsync(Url, Request(password: "wrongpassword"));

        var unknownEmailBody = await unknownEmail.Content.ReadAsStringAsync();
        var wrongPasswordBody = await wrongPassword.Content.ReadAsStringAsync();

        unknownEmail.StatusCode.ShouldBe(wrongPassword.StatusCode);
        unknownEmailBody.ShouldBe(wrongPasswordBody);
    }

    [Fact]
    public async Task Login_WhenEmailIsMissing_Returns400()
    {
        var response = await _client.PostAsJsonAsync(Url, Request(email: ""));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await ReadProblem(response)).Title.ShouldBe("Validation failed");
    }

    private static LoginRequest Request(
        string email = "ada@example.com",
        string password = "correcthorse"
    ) => new(email, password);

    private static async Task<ProblemDetails> ReadProblem(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ProblemDetails>())!;
}
