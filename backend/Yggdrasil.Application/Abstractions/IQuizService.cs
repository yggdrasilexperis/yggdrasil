using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Application.Abstractions;

public interface IQuizService
{
    Task<QuizResponse> CreateQuizAsync(CreateQuizRequest request, CancellationToken cancellationToken);
    Task<QuizResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<QuizResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<QuizResponse> UpdateQuizAsync(Guid id, UpdateQuizRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
