using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Yggdrasil.Api.Filters;
using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Api.Endpoints;

public static class QuizEndpoints
{
    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/quizzes").WithTags("Quizzes");
        group.MapGet("/GetAllQuizzes", GetAll);
        group.MapGet("GetSingleQuiz/{id:guid}", GetById);
        group.MapPost("/CreateQuiz", Create).RequireAuthorization().AddEndpointFilter<ValidationFilter<CreateQuizRequest>>();
        group.MapPut("UpdateQuiz/{id:guid}", Update).RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdateQuizRequest>>();
        group.MapGet("GetComments/{id:guid}/comments", GetComments);
        group.MapGet("GetQuestions/{id:guid}/questions", GetQuestions);
        group.MapDelete("DeleteQuiz/{id:guid}", Delete).RequireAuthorization();

        return app;
    }

    private static async Task<Ok<IEnumerable<QuizResponse>>> GetAll(
        IQuizService service,
        CancellationToken cancellationToken
    )
    {
        var response = await service.GetAllAsync(cancellationToken);
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

