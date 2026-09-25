using Shouldly;

using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Application.Validation;
using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Tests.Unit.Validation;

public sealed class UpdateQuizRequestValidatorTests
{
    private readonly UpdateQuizRequestValidator _sut = new();

    private static UpdateQuizRequest QuizWith(params Guid[] categoryIds) =>
        new("Capitals", "Name the capital", Difficulty.Normal, categoryIds);

    [Fact]
    public void Validate_WhenACategoryIsGiven_Passes() =>
        _sut.Validate(QuizWith(Guid.NewGuid())).IsValid.ShouldBeTrue();

    /// <summary>QuizService attaches Uncategorized instead, so no category is a valid request.</summary>
    [Fact]
    public void Validate_WhenCategoryIdsIsEmpty_Passes() =>
        _sut.Validate(QuizWith()).IsValid.ShouldBeTrue();
}
