using FluentValidation;

using Yggdrasil.Application.Contracts.Authentication;

namespace Yggdrasil.Application.Validation;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(loginRequest => loginRequest.Email)
            .NotEmpty()
            .WithMessage("Email is required");

        RuleFor(loginRequest => loginRequest.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
