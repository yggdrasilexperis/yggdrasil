using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Yggdrasil.Api.Filters;
using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Api.Endpoints;

public static class QuizEndpoints
{
    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/quizzes").WithTags("Quizzes");

        // List quizzes, paged, sortable and filterable by category
        group.MapGet("/", GetPaged)
            .AddEndpointFilter<ValidationFilter<GetQuizzesRequest>>();

        // Get one quiz with its content
        group.MapGet("/{id:guid}", GetById);

        // Create a quiz owned by the signed-in user
        group.MapPost("/", Create)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<CreateQuizRequest>>();

        // Update a quiz. Owner or admin only
        group.MapPut("/{id:guid}", Update)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdateQuizRequest>>();

        // List a quiz's comments
        group.MapGet("/{id:guid}/comments", GetComments);

        // List a quiz's questions
        group.MapGet("/{id:guid}/questions", GetQuestions);

        // Delete a quiz. Owner or admin only
        group.MapDelete("/{id:guid}", Delete)
            .RequireAuthorization();

        return app;
    }

    private static async Task<Ok<PagedResult<QuizResponse>>> GetPaged(
        [AsParameters] GetQuizzesRequest request,
        IQuizService quizService,
        CancellationToken cancellationToken
    )
    {
        var response = await quizService.GetPagedAsync(request, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Created<QuizResponse>> Create(
        CreateQuizRequest request,
        IQuizService quizService,
        CancellationToken cancellationToken)
    {
        var response = await quizService.CreateQuizAsync(request, cancellationToken);
        return TypedResults.Created((string?)null, response);
    }

    private static async Task<Ok<QuizContentResponse>> GetById(
        Guid id,
        IQuizService quizService,
        CancellationToken cancellationToken
    )
    {
        var response = await quizService.GetByIdAsync(id, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<QuizResponse>> Update(
        Guid id,
        UpdateQuizRequest request,
        IQuizService quizService,
        CancellationToken cancellationToken)
    {
        var response = await quizService.UpdateQuizAsync(id, request, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<NoContent> Delete(
        Guid id,
        IQuizService quizService,
        CancellationToken cancellationToken
    )
    {
        await quizService.DeleteAsync(id, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<Ok<IEnumerable<CommentResponse>>> GetComments(
        Guid id,
        IQuizService quizService,
        CancellationToken cancellationToken)
    {
        var response = await quizService.GetCommentsAsync(id, cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<IEnumerable<QuestionResponse>>> GetQuestions(
        Guid id,
        IQuizService quizService,
        CancellationToken cancellationToken)
    {
        var response = await quizService.GetQuestionsAsync(id, cancellationToken);
        return TypedResults.Ok(response);
    }
}

