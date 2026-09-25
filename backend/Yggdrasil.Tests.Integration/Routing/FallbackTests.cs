using System.Net;

using Shouldly;

using Yggdrasil.Tests.Integration.Fixtures;

namespace Yggdrasil.Tests.Integration.Routing;

[Collection(ApiCollection.Name)]
public sealed class FallbackTests(ApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [InlineData("/")]
    [InlineData("/quizzes/0a1b7f2c-0000-4000-8000-000000000204")]
    [InlineData("/quizzes/new")]
    public async Task Get_FrontendRoute_ReturnsIndexHtml(string path)
    {
        var response = await _client.GetAsync(path);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.ShouldBe("text/html");
    }

    [Theory]
    [InlineData("/api/does-not-exist")]
    [InlineData("/api/quizzes/not-a-guid")]
    public async Task Get_UnknownApiRoute_ReturnsNotFound(string path)
    {
        var response = await _client.GetAsync(path);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_MissingFile_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/assets/missing.js");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
