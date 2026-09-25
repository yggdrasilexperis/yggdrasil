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

    public async Task<PagedResult<Quiz>> GetPagedAsync(
        GetQuizzesRequest req,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Quizzes.AsNoTracking().Include(q => q.Categories).AsQueryable();
        var slugs = (req.CategorySlugs ?? [])
            .Where(slug => !string.IsNullOrWhiteSpace(slug))
            .Distinct();

        foreach (var slug in slugs)
            query = query.Where(q => q.Categories.Any(c => c.Slug == slug));

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
            .Include(q => q.Questions.OrderBy(question => question.CreatedAt))
            .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == quizId, cancellationToken);
    }


    public Task<List<Question>> GetQuestionsByQuizIdAsync(Guid quizId, CancellationToken cancellationToken) =>
        _dbContext.Questions
            .Where(q => q.QuizId == quizId)
            .Include(q => q.AnswerOptions)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<Guid?> GetOwnerIdAsync(Guid quizId, CancellationToken cancellationToken) =>
        _dbContext.Quizzes
            .Where(q => q.Id == quizId)
            .Select(q => (Guid?)q.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddQuestionAsync(Question question, CancellationToken cancellationToken)
    {
        _dbContext.Questions.Add(question);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Question?> GetQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken) =>
        _dbContext.Questions
            .FirstOrDefaultAsync(q => q.QuizId == quizId && q.Id == questionId, cancellationToken);

    public async Task DeleteQuestionAsync(Question question, CancellationToken cancellationToken)
    {
        _dbContext.Questions.Remove(question);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Comment?> GetCommentAsync(Guid quizId, Guid commentId, CancellationToken cancellationToken) =>
        _dbContext.Comments
            .FirstOrDefaultAsync(c => c.QuizId == quizId && c.Id == commentId,
                cancellationToken: cancellationToken);

    public Task<List<Comment>> GetCommentsByQuizIdAsync(Guid quizId, CancellationToken cancellationToken) =>
        _dbContext.Comments
            .Where(c => c.QuizId == quizId)
            .ToListAsync(cancellationToken);

    public async Task AddCommentAsync(Comment comment, CancellationToken cancellationToken)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCommentAsync(Comment comment, CancellationToken cancellationToken)
    {
        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCommentAsync(Comment comment, CancellationToken cancellationToken)
    {
        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }


    public Task<Quiz?> GetByQuizTitleAsync(string quizName, CancellationToken cancellationToken)
    {
        return _dbContext.Quizzes.Include(q => q.Categories)
            .FirstOrDefaultAsync(q => q.Title == quizName, cancellationToken);
    }
}
