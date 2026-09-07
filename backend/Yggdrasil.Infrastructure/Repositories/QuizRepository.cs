using Microsoft.EntityFrameworkCore;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Infrastructure.Persistence;

namespace Yggdrasil.Infrastructure.Repositories;

public class QuizRepository(YggdrasilDbContext dbContext) : IQuizRepository
{
    private readonly YggdrasilDbContext _dbContext = dbContext;

    public async Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        _dbContext.Quizzes.Update(quiz);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        _dbContext.Quizzes.Add(quiz);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid quizId, CancellationToken cancellationToken)
    {
        Quiz? quiz = await GetByQuizIdAsync(quizId, cancellationToken);
        if (quiz == null)
            return;
        _dbContext.Quizzes.Remove(quiz);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Quiz>> GetAllQuizzesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Quizzes.Include(q => q.Categories)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Quiz>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Quizzes.Include(q => q.Categories)
            .Where(q => q.OwnerId == userId).ToListAsync(cancellationToken);
    }

    public Task<Quiz?> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken)
    {
        return _dbContext.Quizzes
            .Include(q => q.Categories)
            .Include(q => q.Questions)
            .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == quizId, cancellationToken);
    }

    public Task<Quiz?> GetByQuizTitleAsync(string quizName, CancellationToken cancellationToken)
    {
        return _dbContext.Quizzes.Include(q => q.Categories)
            .FirstOrDefaultAsync(q => q.Title == quizName, cancellationToken);
    }
}
