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

    [Fact]
    public void Validate_WhenCategoryIdsIsEmpty_FailsOnCategoryIds()
    {
        var result = _sut.Validate(QuizWith());

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateQuizRequest.CategoryIds));
    }
}
