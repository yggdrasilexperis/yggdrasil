using Yggdrasil.Domain.Entities;

namespace Yggdrasil.Application.Abstractions;

public interface IQuizRepository
{
    Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken);
    Task CreateAsync(Quiz quiz, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Quiz>> GetAllQuizzesAsync(CancellationToken cancellationToken);
    Task<List<Quiz>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Quiz?> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken);
    Task<Quiz?> GetByQuizTitleAsync(string quizName, CancellationToken cancellationToken);
}
