using Microsoft.EntityFrameworkCore;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Infrastructure.Persistence;

namespace Yggdrasil.Infrastructure.Repositories;

public class QuizRepository(YggdrasilDbContext dbContext) : IQuizRepository
{
    private readonly YggdrasilDbContext _dbContext = dbContext;

    public async Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        _dbContext.Quizzes.Add(quiz);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        _dbContext.Quizzes.Remove(quiz);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // ** Deprecated **
    // public Task<List<Quiz>> GetAllQuizzesAsync(CancellationToken cancellationToken)
    // {
    //     return _dbContext.Quizzes.Include(q => q.Categories)
    //         .ToListAsync(cancellationToken);
    // }

    public async Task<PagedResult<Quiz>> GetPagedAsync(
        GetQuizzesRequest req,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Quizzes.Include(q => q.Categories).AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.CategorySlug))
            query = query.Where(q => q.Categories.Any(c => c.Slug == req.CategorySlug));

        var totalCount = await query.CountAsync(cancellationToken);

        IOrderedQueryable<Quiz> ordered = (req.SortBy, req.SortDirection) switch
        {
            (QuizSortField.Title, SortDirection.Ascending) =>
                query.OrderBy(q => q.Title).ThenBy(q => q.Id),
            (QuizSortField.Title, SortDirection.Descending) =>
                query.OrderByDescending(q => q.Title).ThenBy(q => q.Id),
            (QuizSortField.CreatedAt, SortDirection.Ascending) =>
                query.OrderBy(q => q.CreatedAt).ThenBy(q => q.Id),
            _ => query.OrderByDescending(q => q.CreatedAt).ThenBy(q => q.Id),
        };

        var items = await ordered
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Quiz>(items, req.Page, req.PageSize, totalCount);
    }

    public Task<bool> QuizExistsAsync(Guid quizId, CancellationToken cancellationToken) =>
        _dbContext.Quizzes.AnyAsync(q => q.Id == quizId, cancellationToken);


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

    public Task<List<Question>> GetQuestionsByQuizIdAsync(Guid quizId, CancellationToken cancellationToken) =>
        _dbContext.Questions
            .Where(q => q.QuizId == quizId)
            .Include(q => q.AnswerOptions)
            .ToListAsync(cancellationToken);

    public Task<List<Comment>> GetCommentsByQuizIdAsync(Guid quizId, CancellationToken cancellationToken) =>
        _dbContext.Comments
            .Where(c => c.QuizId == quizId)
            .ToListAsync(cancellationToken);


    public Task<Quiz?> GetByQuizTitleAsync(string quizName, CancellationToken cancellationToken)
    {
        return _dbContext.Quizzes.Include(q => q.Categories)
            .FirstOrDefaultAsync(q => q.Title == quizName, cancellationToken);
    }
}
