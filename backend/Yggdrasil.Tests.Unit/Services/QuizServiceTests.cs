using Microsoft.Extensions.Logging;

using NSubstitute;

using Shouldly;

using Yggdrasil.Application.Abstractions;
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

    private readonly IQuizRepository _quizRepository = Substitute.For<IQuizRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly QuizService _sut;

    public QuizServiceTests()
    {
        _sut = new QuizService(
            _quizRepository,
            _categoryRepository,
            Substitute.For<ILogger<QuizService>>(),
            _currentUser
        );
    }

    private static Quiz OwnedQuiz() => new()
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

        await Should.ThrowAsync<ForbiddenException>(
            () => _sut.UpdateQuizAsync(QuizId, request, CancellationToken.None)
        );

        await _quizRepository.DidNotReceive().UpdateAsync(Arg.Any<Quiz>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsNotOwner_ThrowsForbidden()
    {
        _quizRepository.GetByQuizIdAsync(QuizId, Arg.Any<CancellationToken>()).Returns(OwnedQuiz());
        _currentUser.UserId.Returns(OtherUserId);
        _currentUser.IsInRole(Roles.Admin).Returns(false);

        await Should.ThrowAsync<ForbiddenException>(
            () => _sut.DeleteAsync(QuizId, CancellationToken.None)
        );

        await _quizRepository.DidNotReceive().DeleteAsync(Arg.Any<Quiz>(), Arg.Any<CancellationToken>());
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
}
