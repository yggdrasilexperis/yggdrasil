using Microsoft.Extensions.Logging;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Application.Exceptions;
using Yggdrasil.Domain.Constants;
using Yggdrasil.Domain.Entities;

namespace Yggdrasil.Application.Services;

public class QuizService(
    IQuizRepository quizRepository,
    ICategoryRepository categoryRepository,
    ILogger<QuizService> logger,
    ICurrentUser currentUser,
    IUserLookupService userLookupService
) : IQuizService
{
    public async Task<QuizResponse> CreateQuizAsync(
        CreateQuizRequest request,
        CancellationToken cancellationToken
    )
    {
        var categories = await ResolveCategoriesAsync(request.CategoryIds, cancellationToken);

        var quiz = new Quiz
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            OwnerId = currentUser.UserId,
            Difficulty = request.Difficulty,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Categories = categories,
        };
        await quizRepository.CreateAsync(quiz, cancellationToken);

        return ToResponse(quiz);
    }

    public async Task<QuizContentResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var quiz = await quizRepository.GetByQuizIdAsync(id, cancellationToken);
        if (quiz == null)
        {
            logger.LogWarning("Quiz with id {id} not found", id);
            throw new NotFoundException("Quiz", id);
        }

        var comments = await quizRepository.GetCommentsByQuizIdAsync(id, cancellationToken);
        var usernames = await userLookupService.GetUserNamesAsync(
            comments.Select(c => c.AuthorId), cancellationToken);

        return new QuizContentResponse(
            ToResponse(quiz),
            quiz.Questions.Select(ToResponse),
            comments.Select(comment => new CommentResponse(
                comment.Id,
                comment.AuthorId,
                usernames.GetValueOrDefault(comment.AuthorId, "Unknown user"),
                comment.Body,
                comment.CreatedAt,
                comment.UpdatedAt
            ))
        );
    }

    public async Task<PagedResult<QuizResponse>> GetPagedAsync(
        GetQuizzesRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await quizRepository.GetPagedAsync(request, cancellationToken);
        var items = result.Items.Select(ToResponse).ToList();

        return new PagedResult<QuizResponse>(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount
        );
    }

    public async Task<QuizResponse> UpdateQuizAsync(
        Guid id,
        UpdateQuizRequest request,
        CancellationToken cancellationToken
    )
    {
        var quiz = await quizRepository.GetByQuizIdAsync(id, cancellationToken);
        if (quiz == null)
        {
            logger.LogWarning("Quiz with id {id} not found", id);
            throw new NotFoundException("Quiz", id);
        }
        if (quiz.OwnerId != currentUser.UserId && !currentUser.IsInRole(Roles.Admin))
        {
            throw new ForbiddenException("update this quiz");
        }

        var categories = await ResolveCategoriesAsync(request.CategoryIds, cancellationToken);

        quiz.Title = request.Title;
        quiz.Description = request.Description;
        quiz.Difficulty = request.Difficulty;
        quiz.UpdatedAt = DateTimeOffset.UtcNow;
        quiz.Categories = categories;

        await quizRepository.UpdateAsync(quiz, cancellationToken);

        return ToResponse(quiz);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        // will do two lookups, need to improve
        var quiz = await quizRepository.GetByQuizIdAsync(id, cancellationToken);
        if (quiz == null)
            return;

        if (quiz.OwnerId != currentUser.UserId && !currentUser.IsInRole(Roles.Admin))
        {
            throw new ForbiddenException("delete this quiz");
        }

        await quizRepository.DeleteAsync(quiz, cancellationToken);
    }

    public async Task<IEnumerable<CommentResponse>> GetCommentsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var exists = await quizRepository.QuizExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Quiz", id);
        }
        var comments = await quizRepository.GetCommentsByQuizIdAsync(id, cancellationToken);
        var usernames = await userLookupService.GetUserNamesAsync(
            comments.Select(c => c.AuthorId), cancellationToken);

        return comments.Select(c => new CommentResponse(
            c.Id,
            c.AuthorId,
            usernames.GetValueOrDefault(c.AuthorId, "Unknown user"),
            c.Body,
            c.CreatedAt,
            c.UpdatedAt
        ));
    }

    public async Task<IEnumerable<QuestionResponse>> GetQuestionsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var exists = await quizRepository.QuizExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Quiz", id);
        }
        var questions = await quizRepository.GetQuestionsByQuizIdAsync(id, cancellationToken);

        return questions.Select(ToResponse);
    }

    public async Task<QuestionResponse> AddQuestionAsync(
        Guid quizId,
        CreateQuestionRequest request,
        CancellationToken cancellationToken
    )
    {
        await EnsureCanModifyAsync(quizId, "add questions to this quiz", cancellationToken);

        var question = new Question
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow,
            AnswerOptions = request
                .AnswerOptions.Select(option => new AnswerOption
                {
                    Id = Guid.NewGuid(),
                    Text = option.Text,
                    IsCorrect = option.IsCorrect,
                    CreatedAt = DateTimeOffset.UtcNow,
                })
                .ToList(),
        };

        await quizRepository.AddQuestionAsync(question, cancellationToken);

        return ToResponse(question);
    }

    public async Task<CommentResponse> UpdateCommentAsync(Guid quizId, Guid commentId, UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await quizRepository.GetCommentAsync(quizId, commentId, cancellationToken);

        if (comment == null)
            throw new NotFoundException("Comment", commentId);

        if (comment.AuthorId != currentUser.UserId)
        {
            throw new ForbiddenException("update this comment");
        }

        comment.Body = request.Body;
        comment.UpdatedAt = DateTimeOffset.UtcNow;

        await quizRepository.UpdateCommentAsync(comment, cancellationToken);

        var usernames = await userLookupService.GetUserNamesAsync([comment.AuthorId], cancellationToken);

        return new CommentResponse(
            comment.Id,
            comment.AuthorId,
            usernames.GetValueOrDefault(comment.AuthorId, "Unknown user"),
            comment.Body,
            comment.CreatedAt,
            comment.UpdatedAt
        );
    }

    public async Task<CommentResponse> AddCommentAsync(Guid quizId, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var exists = await quizRepository.QuizExistsAsync(quizId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Quiz", quizId);
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            AuthorId = currentUser.UserId,
            Body = request.Body,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await quizRepository.AddCommentAsync(comment, cancellationToken);

        return new CommentResponse(
            comment.Id,
            comment.AuthorId,
            currentUser.UserName,
            comment.Body,
            comment.CreatedAt,
            comment.UpdatedAt
        );

    }

    public async Task DeleteCommentAsync(Guid quizId, Guid commentId, CancellationToken cancellationToken)
    {
        var comment = await quizRepository.GetCommentAsync(quizId, commentId, cancellationToken);

        if (comment == null)
            throw new NotFoundException("Comment", commentId);

        if (comment.AuthorId != currentUser.UserId && !currentUser.IsInRole(Roles.Admin))
        {
            throw new ForbiddenException("delete comment from this quiz");
        }

        await quizRepository.DeleteCommentAsync(comment, cancellationToken);
    }

    public async Task DeleteQuestionAsync(
        Guid quizId,
        Guid questionId,
        CancellationToken cancellationToken
    )
    {
        await EnsureCanModifyAsync(quizId, "delete questions from this quiz", cancellationToken);

        var question = await quizRepository.GetQuestionAsync(quizId, questionId, cancellationToken);
        if (question == null)
        {
            logger.LogWarning("Question with id {id} not found", questionId);
            throw new NotFoundException("Question", questionId);
        }

        await quizRepository.DeleteQuestionAsync(question, cancellationToken);
    }

    public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(
        CancellationToken cancellationToken
    )
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => new CategoryResponse(c.Id, c.Name, c.Slug));
    }

    private async Task EnsureCanModifyAsync(
        Guid quizId,
        string action,
        CancellationToken cancellationToken
    )
    {
        var ownerId = await quizRepository.GetOwnerIdAsync(quizId, cancellationToken);
        if (ownerId == null)
        {
            logger.LogWarning("Quiz with id {id} not found", quizId);
            throw new NotFoundException("Quiz", quizId);
        }
        if (ownerId != currentUser.UserId && !currentUser.IsInRole(Roles.Admin))
        {
            throw new ForbiddenException(action);
        }
    }

    /// <summary>
    /// Looks the requested categories up, and falls back to Uncategorized when none were asked
    /// for, so that every quiz stays findable by category.
    /// </summary>
    private async Task<List<Category>> ResolveCategoriesAsync(
        ICollection<Guid> categoryIds,
        CancellationToken cancellationToken
    )
    {
        if (categoryIds.Count == 0)
        {
            var fallback = await categoryRepository.GetBySlugAsync(
                CategorySlugs.Uncategorized,
                cancellationToken
            );
            if (fallback == null)
            {
                logger.LogError("The {slug} category is missing", CategorySlugs.Uncategorized);
                throw new BadRequestException(
                    "category_not_found",
                    $"The '{CategorySlugs.Uncategorized}' category is missing. Seed the database."
                );
            }

            return [fallback];
        }

        var categories = await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        if (categories.Count != categoryIds.Distinct().Count())
        {
            var missing = categoryIds.Except(categories.Select(category => category.Id)).ToList();
            logger.LogWarning("Category id(s) {ids} not found", missing);
            throw new BadRequestException(
                "category_not_found",
                $"Category id(s) not found: {string.Join(", ", missing)}"
            );
        }

        return categories;
    }

    private static QuestionResponse ToResponse(Question question)
    {
        return new QuestionResponse(
            question.Id,
            question.Text,
            question.AnswerOptions.Select(answer => new AnswerOptionResponse(
                answer.Id,
                answer.Text,
                answer.IsCorrect
            ))
        );
    }

    // Private helper to create a QuizResponse
    private static QuizResponse ToResponse(Quiz quiz)
    {
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
};
