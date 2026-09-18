using Microsoft.AspNetCore.Http.HttpResults;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("api/categories").WithTags("Categories").MapGet("/", GetAll);
        return app;
    }

    private static async Task<Ok<IEnumerable<CategoryResponse>>> GetAll(
        IQuizService quizService,
        CancellationToken cancellationToken
    )
    {
        var response = await quizService.GetCategoriesAsync(cancellationToken);
        return TypedResults.Ok(response);
    }
}
