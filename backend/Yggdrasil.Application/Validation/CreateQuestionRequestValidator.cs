using FluentValidation;

using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Application.Validation;

public sealed class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Text is required")
            .MaximumLength(1000)
            .WithMessage("Text must be 1000 characters or fewer");

        RuleFor(request => request.AnswerOptions)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("AnswerOptions is required")
            .Must(options => options.Count >= 2)
            .WithMessage("A question needs at least two answer options")
            .Must(options => options.Any(option => option is { IsCorrect: true }))
            .WithMessage("At least one answer option must be correct");

        RuleForEach(request => request.AnswerOptions)
            .NotNull()
            .WithMessage("Answer option cannot be null")
            .ChildRules(option =>
            {
                option.RuleFor(answerOption => answerOption.Text)
                    .NotEmpty()
                    .WithMessage("Answer option text is required")
                    .MaximumLength(500)
                    .WithMessage("Answer option text must be 500 characters or fewer");
            });
    }
}
