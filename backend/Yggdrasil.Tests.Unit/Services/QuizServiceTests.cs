using Microsoft.Extensions.Logging;

using NSubstitute;

using Shouldly;

using Yggdrasil.Application.Abstractions;
using Yggdrasil.Application.Contracts;
using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Application.Exceptions;
using Yggdrasil.Application.Services;
using Yggdrasil.Domain.Constants;
using Yggdrasil.Domain.Entities;
using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Tests.Unit.Services;

public sealed class QuizServiceTests
{
    private static readonly Guid OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid AdminId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid QuizId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid QuestionId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private static readonly Guid CommentId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    private static Comment OwnedComment() => new()
    {
        Id = CommentId,
        QuizId = QuizId,
        AuthorId = OwnerId,
        Body = "Original body",
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    private readonly IQuizRepository _quizRepository = Substitute.For<IQuizRepository>();
    private readonly ICategoryRepository _categoryRepository =
        Substitute.For<ICategoryRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUserLookupService _lookUp = Substitute.For<IUserLookupService>();
    private readonly QuizService _sut;

    public QuizServiceTests()
    {
        _sut = new QuizService(
            _quizRepository,
            _categoryRepository,
            Substitute.For<ILogger<QuizService>>(),
            _currentUser,
            _lookUp
        );
        _categoryRepository
            .GetBySlugAsync(CategorySlugs.Uncategorized, Arg.Any<CancellationToken>())
            .Returns(
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Uncategorized",
                    Slug = CategorySlugs.Uncategorized,
                }
            );
    }

    private static CreateQuestionRequest NewQuestion() =>
        new(
            "Capital of Norway?",
            [
                new CreateAnswerOptionRequest("Oslo", true),
                new CreateAnswerOptionRequest("Bergen", false),
            ]
        );

    private static Quiz OwnedQuiz() =>
        new()
        {
            Id = QuizId,
            Title = "Original title",
            Description = "Original description",
            Difficulty = Difficulty.Normal,
            OwnerId = OwnerId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

    [Fact]
    public async Task UpdateQuizAsync_WhenCallerIsNotOwner_ThrowsForbidden()
    {
        _quizRepository.GetByQuizIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnedQuiz());
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        var request = new UpdateQuizRequest("New title", "New description", Difficulty.Hard, []);

        await Should.ThrowAsync<ForbiddenException>(() =>
            _sut.UpdateQuizAsync(QuizId, request, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Quiz>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsNotOwner_ThrowsForbidden()
    {
        _quizRepository.GetByQuizIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnedQuiz());
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        await Should.ThrowAsync<ForbiddenException>(() =>
            _sut.DeleteAsync(QuizId, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .DeleteAsync(Arg.Any<Quiz>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsAdminButNotOwner_Succeeds()
    {
        var quiz = OwnedQuiz();
        _quizRepository.GetByQuizIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(quiz);
        _currentUser.UserId.Returns(AdminId);
        _currentUser.IsInRole(Roles.Admin).Returns(true);

        await _sut.DeleteAsync(QuizId, CancellationToken.None);

        await _quizRepository.Received(1).DeleteAsync(quiz, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateQuizAsync_WhenCallerIsOwner_Succeeds()
    {
        var quiz = OwnedQuiz();
        _quizRepository.GetByQuizIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(quiz);
        _categoryRepository
            .GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _currentUser.UserId.Returns(OwnerId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        var request = new UpdateQuizRequest("New title", "New description", Difficulty.Hard, []);

        await _sut.UpdateQuizAsync(QuizId, request, CancellationToken.None);

        await _quizRepository.Received(1).UpdateAsync(quiz, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateQuizAsync_WhenCallerIsAdminButNotOwner_Succeeds()
    {
        var quiz = OwnedQuiz();
        _quizRepository.GetByQuizIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(quiz);
        _categoryRepository
            .GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _currentUser.UserId.Returns(AdminId);
        _currentUser.IsInRole(Roles.Admin).Returns(true);

        var request = new UpdateQuizRequest("New title", "New description", Difficulty.Hard, []);

        await _sut.UpdateQuizAsync(QuizId, request, CancellationToken.None);

        await _quizRepository.Received(1).UpdateAsync(quiz, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsOwner_Succeeds()
    {
        var quiz = OwnedQuiz();
        _quizRepository.GetByQuizIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(quiz);
        _currentUser.UserId.Returns(OwnerId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        await _sut.DeleteAsync(QuizId, CancellationToken.None);

        await _quizRepository.Received(1).DeleteAsync(quiz, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedAsync_WhenRepositoryReturnsAPage_MapsEntitiesAndKeepsPagingMetadata()
    {
        var quiz = new Quiz
        {
            Id = Guid.NewGuid(),
            Title = "Existing Quiz",
            Difficulty = Difficulty.Normal,
            OwnerId = Guid.NewGuid(),
            Categories =
            [
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Games",
                    Slug = "games",
                },
            ],
        };
        _quizRepository
            .GetPagedAsync(Arg.Any<GetQuizzesRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Quiz>([quiz], Page: 2, PageSize: 10, TotalCount: 31));

        var result = await _sut.GetPagedAsync(new GetQuizzesRequest(), CancellationToken.None);

        result.Page.ShouldBe(2);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(31);
        // 31 over pages of 10 is the remainder case the frontend's "Next" button depends on
        result.TotalPages.ShouldBe(4);

        var item = result.Items.ShouldHaveSingleItem();
        item.Id.ShouldBe(quiz.Id);
        item.Title.ShouldBe("Existing Quiz");
        item.Categories.ShouldHaveSingleItem().Slug.ShouldBe("games");
    }

    [Fact]
    public async Task AddQuestionAsync_WhenQuizDoesNotExist_ThrowsNotFound()
    {
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns((Guid?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _sut.AddQuestionAsync(QuizId, NewQuestion(), CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .AddQuestionAsync(Arg.Any<Question>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddQuestionAsync_WhenCallerIsNotOwner_ThrowsForbidden()
    {
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnerId);
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        await Should.ThrowAsync<ForbiddenException>(() =>
            _sut.AddQuestionAsync(QuizId, NewQuestion(), CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .AddQuestionAsync(Arg.Any<Question>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddQuestionAsync_WhenCallerIsOwner_SavesTheQuestionWithItsAnswerOptions()
    {
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnerId);
        _currentUser.UserId.Returns(OwnerId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        var response = await _sut.AddQuestionAsync(QuizId, NewQuestion(), CancellationToken.None);

        response.Text.ShouldBe("Capital of Norway?");
        response.AnswerOptions.Count().ShouldBe(2);
        response.AnswerOptions.ShouldContain(option => option.Text == "Oslo" && option.IsCorrect);

        await _quizRepository
            .Received(1)
            .AddQuestionAsync(
                Arg.Is<Question>(question =>
                    question.QuizId == QuizId && question.AnswerOptions.Count == 2
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task AddQuestionAsync_WhenCallerIsAdminButNotOwner_Succeeds()
    {
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnerId);
        _currentUser.UserId.Returns(AdminId);
        _currentUser.IsInRole(Roles.Admin).Returns(true);

        await _sut.AddQuestionAsync(QuizId, NewQuestion(), CancellationToken.None);

        await _quizRepository
            .Received(1)
            .AddQuestionAsync(Arg.Any<Question>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteQuestionAsync_WhenCallerIsNotOwner_ThrowsForbidden()
    {
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnerId);
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        await Should.ThrowAsync<ForbiddenException>(() =>
            _sut.DeleteQuestionAsync(QuizId, QuestionId, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .DeleteQuestionAsync(Arg.Any<Question>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteQuestionAsync_WhenTheQuestionBelongsToAnotherQuiz_ThrowsNotFound()
    {
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnerId);
        _quizRepository
            .GetQuestionAsync(QuizId, QuestionId, Arg.Any<CancellationToken>())
            .Returns((Question?)null);
        _currentUser.UserId.Returns(OwnerId);

        await Should.ThrowAsync<NotFoundException>(() =>
            _sut.DeleteQuestionAsync(QuizId, QuestionId, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .DeleteQuestionAsync(Arg.Any<Question>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteQuestionAsync_WhenCallerIsOwner_Succeeds()
    {
        var question = new Question
        {
            Id = QuestionId,
            QuizId = QuizId,
            Text = "Capital of Norway?",
        };
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnerId);
        _quizRepository
            .GetQuestionAsync(QuizId, QuestionId, Arg.Any<CancellationToken>())
            .Returns(question);
        _currentUser.UserId.Returns(OwnerId);

        await _sut.DeleteQuestionAsync(QuizId, QuestionId, CancellationToken.None);

        await _quizRepository
            .Received(1)
            .DeleteQuestionAsync(question, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCommentAsync_WhenCallerIsNotOwnerOrAdmin_ThrowsForbidden()
    {
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns(OwnedComment());
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        await Should.ThrowAsync<ForbiddenException>(() =>
            _sut.DeleteCommentAsync(QuizId, CommentId, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .DeleteCommentAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCommentAsync_WhenTheCommentBelongsToAnotherQuiz_ThrowsNotFound()
    {
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns((Comment?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _sut.DeleteCommentAsync(QuizId, CommentId, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .DeleteCommentAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCommentAsync_WhenCallerIsAdminButNotOwner_Succeeds()
    {
        var comment = OwnedComment();
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns(comment);
        _currentUser.UserId.Returns(AdminId);
        _currentUser.IsInRole(Roles.Admin).Returns(true);

        await _sut.DeleteCommentAsync(QuizId, CommentId, CancellationToken.None);

        await _quizRepository.Received(1).DeleteCommentAsync(comment, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCommentAsync_WhenCallerIsOwner_Succeeds()
    {
        var comment = OwnedComment();
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns(comment);
        _currentUser.UserId.Returns(OwnerId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        await _sut.DeleteCommentAsync(QuizId, CommentId, CancellationToken.None);

        await _quizRepository.Received(1).DeleteCommentAsync(comment, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteQuestionAsync_WhenCallerIsAdminButNotOwner_Succeeds()
    {
        var question = new Question
        {
            Id = QuestionId,
            QuizId = QuizId,
            Text = "Capital of Norway?",
        };
        _quizRepository.GetOwnerIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnerId);
        _quizRepository
            .GetQuestionAsync(QuizId, QuestionId, Arg.Any<CancellationToken>())
            .Returns(question);
        _currentUser.UserId.Returns(AdminId);
        _currentUser.IsInRole(Roles.Admin).Returns(true);

        await _sut.DeleteQuestionAsync(QuizId, QuestionId, CancellationToken.None);

        await _quizRepository
            .Received(1)
            .DeleteQuestionAsync(question, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCommentAsync_WhenTheCommentBelongsToAnotherQuiz_ThrowsNotFound()
    {
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns((Comment?)null);

        var request = new UpdateCommentRequest("Attempted update");

        await Should.ThrowAsync<NotFoundException>(() =>
            _sut.UpdateCommentAsync(QuizId, CommentId, request, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .UpdateCommentAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCommentAsync_WhenTheCallerIsOwner_Succeeds()
    {
        var comment = OwnedComment();
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns(comment);
        _currentUser.UserId.Returns(OwnerId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);
        _lookUp
            .GetUserNamesAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, string> { [OwnerId] = "owner-username" });

        var request = new UpdateCommentRequest("Updated body");

        var response = await _sut.UpdateCommentAsync(QuizId, CommentId, request, CancellationToken.None);

        response.Body.ShouldBe("Updated body");
        response.AuthorUsername.ShouldBe("owner-username");
        await _quizRepository
            .Received(1)
            .UpdateCommentAsync(Arg.Is<Comment>(c => c.Body == "Updated body"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCommentAsync_WhenTheCallerIsNotOwnerOrAdmin_ThrowsForbidden()
    {
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns(OwnedComment());
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        var request = new UpdateCommentRequest("Attempted update");

        await Should.ThrowAsync<ForbiddenException>(() =>
            _sut.UpdateCommentAsync(QuizId, CommentId, request, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .UpdateCommentAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCommentAsync_WhenCallerIsAdminButNotOwner_ThrowsForbidden()
    {
        _quizRepository.GetCommentAsync(QuizId, CommentId, Arg.Any<CancellationToken>()).Returns(OwnedComment());
        _currentUser.UserId.Returns(AdminId);
        _currentUser.IsInRole(Roles.Admin).Returns(true);

        var request = new UpdateCommentRequest("Attempted admin update");

        await Should.ThrowAsync<ForbiddenException>(() =>
            _sut.UpdateCommentAsync(QuizId, CommentId, request, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .UpdateCommentAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCommentAsync_WhenQuizDoesNotExist_ThrowsNotFound()
    {
        _quizRepository.QuizExistsAsync(QuizId, Arg.Any<CancellationToken>()).Returns(false);

        var request = new CreateCommentRequest("New comment");

        await Should.ThrowAsync<NotFoundException>(() =>
            _sut.AddCommentAsync(QuizId, request, CancellationToken.None)
        );

        await _quizRepository
            .DidNotReceive()
            .AddCommentAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCommentAsync_WhenAnyAuthenticatedCallerAddsAComment_Succeeds()
    {
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.UserName.Returns("commenter");
        _quizRepository.QuizExistsAsync(QuizId, Arg.Any<CancellationToken>()).Returns(true);

        var request = new CreateCommentRequest("New comment");

        var response = await _sut.AddCommentAsync(QuizId, request, CancellationToken.None);

        response.Body.ShouldBe("New comment");
        response.AuthorId.ShouldBe(OtherUserId);
        response.AuthorUsername.ShouldBe("commenter");
        await _quizRepository
            .Received(1)
            .AddCommentAsync(
                Arg.Is<Comment>(c => c.QuizId == QuizId && c.AuthorId == OtherUserId && c.Body == "New comment"),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task CreateQuizAsync_WithNoCategories_AttachesUncategorized()
    {
        var uncategorized = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Uncategorized",
            Slug = CategorySlugs.Uncategorized,
        };
        _currentUser.UserId.Returns(OwnerId);
        _categoryRepository
            .GetBySlugAsync(CategorySlugs.Uncategorized, Arg.Any<CancellationToken>())
            .Returns(uncategorized);

        var response = await _sut.CreateQuizAsync(
            new CreateQuizRequest("Capitals", "Name the capital", Difficulty.Normal, []),
            CancellationToken.None
        );

        response.Categories.ShouldHaveSingleItem().Slug.ShouldBe(CategorySlugs.Uncategorized);
    }

    [Fact]
    public async Task CreateQuizAsync_WhenUncategorizedIsMissing_ThrowsBadRequest()
    {
        _currentUser.UserId.Returns(OwnerId);
        _categoryRepository
            .GetBySlugAsync(CategorySlugs.Uncategorized, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        await Should.ThrowAsync<BadRequestException>(() =>
            _sut.CreateQuizAsync(
                new CreateQuizRequest("Capitals", "Name the capital", Difficulty.Normal, []),
                CancellationToken.None
            )
        );
    }
}
