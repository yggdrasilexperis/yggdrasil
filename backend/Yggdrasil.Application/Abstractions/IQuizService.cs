using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Application.Abstractions;

public interface IQuizService
{
    Task<QuizResponse> CreateQuizAsync(
        CreateQuizRequest request,
        CancellationToken cancellationToken
    );
    Task<QuizContentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<QuizResponse>> GetPagedAsync(GetQuizzesRequest request, CancellationToken cancellationToken);
    Task<QuizResponse> UpdateQuizAsync(Guid id, UpdateQuizRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<CommentResponse>> GetCommentsAsync(
        Guid id,
        CancellationToken cancellationToken
    );
    Task<IEnumerable<QuestionResponse>> GetQuestionsAsync(
        Guid id,
        CancellationToken cancellationToken
    );
    Task<QuestionResponse> AddQuestionAsync(
        Guid quizId,
        CreateQuestionRequest request,
        CancellationToken cancellationToken
    );
    Task<QuestionResponse> UpdateQuestionAsync(
        Guid quizId,
        Guid questionId,
        UpdateQuestionRequest request,
        CancellationToken cancellationToken
    );
    Task DeleteQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken);
    Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken);
}
