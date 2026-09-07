using Microsoft.Extensions.Logging;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Application.Exceptions;
using Yggdrasil.Domain.Entities;

namespace Yggdrasil.Application.Services;

public class QuizService(
    IQuizRepository quizRepository,
    ICategoryRepository categoryRepository,
        ILogger<QuizService> logger
    ) : IQuizService
{
    public async Task<QuizResponse> CreateQuizAsync(CreateQuizRequest request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetByIdsAsync(request.CategoryIds, cancellationToken);
        if (categories.Count != request.CategoryIds.Distinct().Count())
        {
            logger.LogWarning("Category list not found for quiz with id {id} not found", request.CategoryIds);
            throw new NotFoundException("Category", request.CategoryIds);
        }

        var newQuiz = new Quiz
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            OwnerId = Guid.NewGuid(), // will add ownsership interface
            Difficulty = request.Difficulty,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Categories = categories
        };
        await quizRepository.CreateAsync(newQuiz, cancellationToken);

        return new QuizResponse(
            newQuiz.Id,
            newQuiz.Title,
            newQuiz.Description,
            newQuiz.OwnerId,
            newQuiz.Difficulty,
            newQuiz.CreatedAt,
            newQuiz.UpdatedAt,
            newQuiz.Categories.Select(c => new CategoryResponse(c.Id, c.Name, c.Slug))
        );
    }

    public async Task<QuizResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByQuizIdAsync(id, cancellationToken);

        if (quiz != null)
        {
            return new QuizResponse(
                quiz.Id,
                quiz.Title,
                quiz.Description,
                quiz.OwnerId,
                quiz.Difficulty,
                quiz.CreatedAt,
                quiz.UpdatedAt,
                quiz.Categories.Select(c => new CategoryResponse(c.Id, c.Name, c.Slug)));
        }

        logger.LogWarning("Quiz with id {id} not found", id);
        throw new NotFoundException("Quiz", id);

    }

    public async Task<IEnumerable<QuizResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var quizzes = await quizRepository.GetAllQuizzesAsync(cancellationToken);

        return quizzes.Select(q => new QuizResponse(
            q.Id,
            q.Title,
            q.Description,
            q.OwnerId,
            q.Difficulty,
            q.CreatedAt,
            q.UpdatedAt,
            q.Categories.Select(c => new CategoryResponse(c.Id, c.Name, c.Slug))
        ));
    }


    public async Task<QuizResponse> UpdateQuizAsync(Guid id, UpdateQuizRequest request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByQuizIdAsync(id, cancellationToken);
        if (quiz == null)
        {
            logger.LogWarning("Quiz with id {id} not found", id);
            throw new NotFoundException("Quiz", id);
        }
        var categories = await categoryRepository.GetByIdsAsync(request.CategoryIds, cancellationToken);
        if (categories.Count != request.CategoryIds.Distinct().Count())
        {
            // need to improve logging
            logger.LogWarning("Category with id {id} not found", request.CategoryIds);
            throw new NotFoundException("Category", request.CategoryIds);
        }

        quiz.Title = request.Title;
        quiz.Description = request.Description;
        quiz.Difficulty = request.Difficulty;
        quiz.UpdatedAt = DateTimeOffset.UtcNow;
        quiz.Categories = categories;

        await quizRepository.UpdateAsync(quiz, cancellationToken);

        return new QuizResponse(
            quiz.Id,
            quiz.Title,
            quiz.Description,
            quiz.OwnerId,
            quiz.Difficulty,
            quiz.CreatedAt,
            quiz.UpdatedAt,
            quiz.Categories.Select(c => new CategoryResponse(c.Id, c.Name, c.Slug))
        );

    }


    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        // will do two lookups, need to improve
        var quiz = await quizRepository.GetByQuizIdAsync(id, cancellationToken);
        if (quiz == null)
            return;

        await quizRepository.DeleteAsync(id, cancellationToken);
    }
}
