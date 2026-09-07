using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Shouldly;

using Yggdrasil.Application.Contracts.Authentication;
using Yggdrasil.Tests.Integration.Fixtures;

namespace Yggdrasil.Tests.Integration.Auth;

[Collection(ApiCollection.Name)]
public sealed class TokenAcceptanceTests(ApiFactory factory) : IAsyncLifetime
{
    private const string OtherSigningKey = "a-different-key-that-the-api-does-not-know-0123456789";

    private readonly HttpClient _client = factory.CreateClient();

    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ProtectedRoute_WithoutAToken_Returns401()
    {
        var response = await _client.GetAsync(ProtectedProbe.Path);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedRoute_WithTheTokenFromRegister_Returns200WithTheUserId()
    {
        var registration = await _client.PostAsJsonAsync(
            "api/auth/register",
            new RegisterRequest("ada@example.com", "ada_lovelace", "correcthorse")
        );
        var account = (await registration.Content.ReadFromJsonAsync<AuthResponse>())!;

        var response = await GetProtected(account.Token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        (await response.Content.ReadAsStringAsync()).ShouldBe(account.User.Id.ToString());
    }

    [Fact]
    public async Task ProtectedRoute_WithATokenSignedByAnotherKey_Returns401()
    {
        var response = await GetProtected(ForgedToken());

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private Task<HttpResponseMessage> GetProtected(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ProtectedProbe.Path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return _client.SendAsync(request);
    }

    private static string ForgedToken() =>
        new JsonWebTokenHandler().CreateToken(
            new SecurityTokenDescriptor
            {
                Issuer = "Yggdrasil.Api",
                Audience = "Yggdrasil.Client",
                Expires = DateTime.UtcNow.AddMinutes(60),
                Subject = new ClaimsIdentity(
                    [new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString())]
                ),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(OtherSigningKey)),
                    SecurityAlgorithms.HmacSha256
                ),
            }
        );
}
