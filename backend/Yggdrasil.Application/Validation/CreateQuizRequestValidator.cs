using FluentValidation;

using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Application.Validation;

public sealed class CreateQuizRequestValidator : AbstractValidator<CreateQuizRequest>
{
    public CreateQuizRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must be 200 characters or fewer");

        RuleFor(request => request.Description)
            .MaximumLength(2000)
            .WithMessage("Description must be 2000 characters or fewer");

        RuleFor(request => request.Difficulty)
            .IsInEnum()
            .WithMessage("Difficulty must be a valid value");
    }
}
