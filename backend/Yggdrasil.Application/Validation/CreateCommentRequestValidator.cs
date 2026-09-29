using FluentValidation;

using Yggdrasil.Application.Contracts.Quiz;

namespace Yggdrasil.Application.Validation;

public sealed class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(request => request.Body)
            .NotEmpty()
            .WithMessage("Body is required")
            .MaximumLength(2000)
            .WithMessage("Body must be 2000 characters or fewer");
    }
}
