using Shouldly;

using Yggdrasil.Application.Contracts.Quiz;
using Yggdrasil.Application.Validation;

namespace Yggdrasil.Tests.Unit.Validation;

public sealed class CreateQuestionRequestValidatorTests
{
    private readonly CreateQuestionRequestValidator _sut = new();

    private static CreateQuestionRequest QuestionWith(params CreateAnswerOptionRequest[] answerOptions) =>
        new("Capital of Norway?", answerOptions);

    private static CreateQuestionRequest QuestionTitled(string text) =>
        new(text, [new CreateAnswerOptionRequest("Oslo", true), new CreateAnswerOptionRequest("Bergen", false)]);

    [Fact]
    public void Validate_WhenTextAndTwoAnswerOptionsAreGiven_Passes() =>
        _sut.Validate(QuestionTitled("Capital of Norway?")).IsValid.ShouldBeTrue();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenTextIsEmpty_FailsOnText(string text)
    {
        var result = _sut.Validate(QuestionTitled(text));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateQuestionRequest.Text));
    }

    [Fact]
    public void Validate_WhenTextIsLongerThanTheColumn_FailsOnText()
    {
        var result = _sut.Validate(QuestionTitled(new string('a', 1001)));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateQuestionRequest.Text));
    }

    [Fact]
    public void Validate_WhenThereIsOnlyOneAnswerOption_FailsOnAnswerOptions()
    {
        var result = _sut.Validate(QuestionWith(new CreateAnswerOptionRequest("Oslo", true)));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateQuestionRequest.AnswerOptions));
    }

    [Fact]
    public void Validate_WhenNoAnswerOptionIsCorrect_FailsOnAnswerOptions()
    {
        var result = _sut.Validate(QuestionWith(
            new CreateAnswerOptionRequest("Oslo", false),
            new CreateAnswerOptionRequest("Bergen", false)
        ));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateQuestionRequest.AnswerOptions));
    }

    [Fact]
    public void Validate_WhenAnAnswerOptionHasNoText_Fails()
    {
        var result = _sut.Validate(QuestionWith(
            new CreateAnswerOptionRequest("", true),
            new CreateAnswerOptionRequest("Bergen", false)
        ));

        result.IsValid.ShouldBeFalse();
    }
}
